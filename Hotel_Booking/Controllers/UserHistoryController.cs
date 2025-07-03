using DTO;
using Hotel_Booking.ControllerBase;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using Models;

namespace Hotel_Booking.Controllers
{
    [Authorize]
    public class UserHistoryController : MVCControlBase
    {
        public UserHistoryController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IActionResult> GetProfileHistory()
        {
            int id = AccountController.GetUserId(HttpContext);
            if (id != 0)
            {
                var bookingHistory = await _unitOfWork.Bookings.FindAllAsync(u => u.UserId == id, new string[] { "PaymentMethod", "User", "Room", "Room.Hotel" });

                foreach(var book in bookingHistory)
                {
                    if(book.CheckOutDate.Date < DateTime.Now)
                    {
                        book.Room!.AvailabilityStatus = "Available";
                        book.BookingStatus = "Completed";
                    }
                }
                _unitOfWork.Save();
                return View(bookingHistory);
            }

            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var paymentMethods = await _unitOfWork.PaymentMethods.GetAllAsync();
            ViewBag.PaymentMethods = new SelectList(paymentMethods, "Id", "MethodName");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookingDto bookingDto, int id)
        {
            var paymentMethods = await _unitOfWork.PaymentMethods.GetAllAsync();
            ViewBag.PaymentMethods = new SelectList(paymentMethods, "Id", "MethodName");

            if (ModelState.IsValid)
            {
                if (bookingDto.CheckInDate < bookingDto.CheckOutDate &&
                   bookingDto.CheckInDate.Date >= DateTime.Now.Date)
                {
                    var difference = bookingDto.CheckOutDate.Date - bookingDto.CheckInDate.Date;

                    var duration = (int)difference.TotalDays;

                    var booking = _unitOfWork.Bookings.FindOneItem(b => b.Id == id, new string[] { "PaymentMethod", "User", "Room", "Room.Hotel" });

                    ViewBag.RoomNumber = booking.Room!.RoomNumber;

                    booking.CheckInDate = bookingDto.CheckInDate;
                    booking.CheckOutDate = bookingDto.CheckOutDate;
                    booking.TotalPrice = duration * booking.Room!.BasePrice;
                    booking.PaymentMethodId = bookingDto.PaymentMethod;

                    _unitOfWork.Save();


                    return RedirectToAction("GetProfileHistory");
                }
            }
            return View();
        }

        public IActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete(Booking booking, int id)
        {
            booking = _unitOfWork.Bookings.FindOneItem(b => b.Id == id, new string[] { "PaymentMethod", "User", "Room", "Room.Hotel" });
            booking.Room!.AvailabilityStatus = "Available";

            _unitOfWork.Bookings.Delete(booking);


            _unitOfWork.Save();

            return RedirectToAction("GetProfileHistory");

        }

        public IActionResult Rating(int id) 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Rating(ReviewDto reviewDto, int id)
        {
            var booking= _unitOfWork.Bookings.FindOneItem(b => b.Id == id, new string[] { "PaymentMethod", "User", "Room", "Room.Hotel" });

            var userId = AccountController.GetUserId(HttpContext);

            var review = new Review()
            {
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                ReviewDate = DateTime.Now,
                UserId = userId,
                HotelId = booking.Room!.HotelId
            };

            await _unitOfWork.Reviews.AddAsync(review);

            _unitOfWork.Save();

            var hotel = await _unitOfWork.Hotels.GetByIdAsync(review.HotelId??0);

            var hotelReviewsCount = _unitOfWork.Reviews.Count(r => r.HotelId == hotel.Id);
            if(hotelReviewsCount > 0)
            {
                var sumOfReviews = _unitOfWork.Reviews.RatingSummation(hotel.Id);

                var newRatingOfHotel = sumOfReviews / hotelReviewsCount;

                hotel.StarRating = newRatingOfHotel;
                _unitOfWork.Save();
            }
            
            return RedirectToAction("GetProfileHistory");
        }

    }
}
