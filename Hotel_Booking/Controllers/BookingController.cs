using Business_Logic_Services;
using DTO;
using Hotel_Booking.ControllerBase;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Models;
using Stripe;
using System.IdentityModel.Tokens.Jwt;

namespace Hotel_Booking.Controllers
{

    public class BookingController : MVCControlBase
    {
        private readonly EmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public BookingController(IUnitOfWork unitOfWork, EmailSender emailSender, IConfiguration configuration) : base(unitOfWork)
        {
            _emailSender = emailSender;
            _configuration = configuration;
        }

        public async Task<IActionResult> BookingProcess()
        {
            var paymentMethods = await _unitOfWork.PaymentMethods.GetAllAsync();
            ViewBag.PaymentMethods = new SelectList(paymentMethods, "Id", "MethodName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BookingProcess(BookingDto bookingDto, int id, string stripeToken)
        {
            var paymentMethods = await _unitOfWork.PaymentMethods.GetAllAsync();
            ViewBag.PaymentMethods = new SelectList(paymentMethods, "Id", "MethodName");
            ViewBag.Publishablekey = _configuration["Stripe:Publishablekey"];

            if (ModelState.IsValid)
            {
                if (bookingDto.CheckInDate < bookingDto.CheckOutDate.Date &&
                   bookingDto.CheckInDate.Date >= DateTime.Now.Date)
                {
                    var room = _unitOfWork.Rooms.FindOneItem(r => r.Id == id);

                    int userId = AccountController.GetUserId(HttpContext);
                    var user = await _unitOfWork.Users.GetByIdAsync(userId);

                    var difference = bookingDto.CheckOutDate.Date - bookingDto.CheckInDate.Date;

                    var duration = (int)difference.TotalDays;
                    var totalPrice = duration * room.BasePrice;
                    try
                    {
                        var chargeOption = new ChargeCreateOptions()
                        {
                            Amount = (long)totalPrice * 100,
                            Currency = "usd",
                            Description = $"Hotel Booking for room {room.RoomNumber}",
                            Source= stripeToken,
                            ReceiptEmail = user.Email
                        };

                        var client = new Stripe.StripeClient(_configuration["Stripe:SecretKey"]);
                        var chargeService = new ChargeService(client);
                        var charge = await chargeService.CreateAsync(chargeOption);

                        if (charge.Status != "succeeded")
                        {
                            ModelState.AddModelError("", "Payment failed");
                            return View(bookingDto);
                        }
                        var booking = new Booking();


                        booking.BookingDate = DateTime.Now;
                        booking.CheckInDate = bookingDto.CheckInDate;
                        booking.CheckOutDate = bookingDto.CheckOutDate;
                        booking.TotalPrice = totalPrice;
                        booking.PaymentStatus = "Paid";
                        booking.BookingStatus = "Confirmed";
                        booking.PaymentMethodId = bookingDto.PaymentMethod;
                        booking.RoomId = id;
                        booking.UserId = userId;

                        await _unitOfWork.Bookings.AddAsync(booking);

                        room.AvailabilityStatus = "Occupied";
                        ViewBag.RoomNumber = room.RoomNumber;

                        _unitOfWork.Rooms.Update(room);
                        _unitOfWork.Save();


                        var hotel = await _unitOfWork.Hotels.GetByIdAsync((int)room.HotelId!);

                        string subject = "booking confirmation";
                        string username = user.FirstName + " " + user.LastName;


                        string message = $"Dear {username},\n\n" +
                                         "Thank you for choosing our hotel!\n" +
                                         "We are pleased to confirm your booking.\n\n" +
                                         "Booking Details:\n" +
                                        $"Hotel Name: {hotel.Name}\n" +
                                        $"Room Number: {room.RoomNumber}\n" +
                                        $"Check-in Date: {booking.CheckInDate:dddd, MMMM d, yyyy}\n" +
                                        $"Check-out Date: {booking.CheckOutDate:dddd, MMMM d, yyyy}\n\n" +
                                         "If you have any questions or special requests, feel free to contact us.\n\n" +
                                         "We look forward to welcoming you!\n\n" +
                                         "Best regards,\n" +
                                         "The Hotel Team";


                        //sending email to the user for booking confirmation

                        //await _emailSender.SendEmail(subject, user.Email, username, message);

                        return RedirectToAction("GetProfileHistory", "UserHistory");

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Payment exception: " + ex.Message);
                        return View(bookingDto);
                    }
                }
            }
            ModelState.AddModelError("", "You must choose a correct Date");
            return View(bookingDto);
        }


    }
}
