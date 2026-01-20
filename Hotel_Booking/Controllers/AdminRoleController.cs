using DTO;
using Hotel_Booking.ControllerBase;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.Identity.Client;
using Models;

namespace Hotel_Booking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminRoleController : MVCControlBase
    {
        private readonly IWebHostEnvironment env;

        public AdminRoleController(IUnitOfWork unitOfWork, IWebHostEnvironment env) : base(unitOfWork)
        {
            this.env = env;
        }

		#region for Dashboard
		public async Task<IActionResult> Dashboard()
        {
            var bookingInfo = await _unitOfWork.Bookings.GetAllAsync();

            var dashboardInfo = new DashboardContentDto();

            dashboardInfo.TotalHotels = _unitOfWork.Hotels.Count(x => true);
            dashboardInfo.ActiveRooms = _unitOfWork.Rooms.Count(r => r.AvailabilityStatus == "Occupied");

            foreach (var bookingPrice in bookingInfo)
            {
                dashboardInfo.Revenue += bookingPrice.TotalPrice;
            }

            var bookings = await _unitOfWork.Bookings.FindAllAsync(b => b.CheckOutDate > DateTime.Now, new string[] { "PaymentMethod", "User", "Room", "Room.Hotel" });
            dashboardInfo.BookingsInfo = bookings;

            return View(dashboardInfo);
        }
        #endregion

        #region CRUD of Hotel
        public async Task<IActionResult> ShowHotels()
        {
            var hotels = await _unitOfWork.Hotels.GetAllAsync();
            return View(hotels);
        }

        public IActionResult AddHotel()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddHotel(HotelDto hotelDto)
        {
            if (ModelState.IsValid)
            {
                if (hotelDto.ImageFile == null && hotelDto.ImageFile?.Length < 0)
                {
                    ModelState.AddModelError("", "You must choose an image");
                    return View(hotelDto);
                }

                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(hotelDto.ImageFile?.FileName);

                var folderName = env.WebRootPath + "/Images/Hotels/";

                using (var stream = System.IO.File.Create(folderName + imageName))
                {
                    hotelDto.ImageFile!.CopyTo(stream);
                }

                var hotel = new Hotel()
                {
                    Name = hotelDto.Name,
                    Address = hotelDto.Address,
                    City = hotelDto.City,
                    Country = hotelDto.Country,
                    PhoneNumber = hotelDto.PhoneNumber,
                    Email = hotelDto.Email,
                    Description = hotelDto.Description,
                    ImageUrl = imageName
                };

                await _unitOfWork.Hotels.AddAsync(hotel);
                _unitOfWork.Save();

                return RedirectToAction("ShowHotels");
            }

            return View(hotelDto);

        }

        public async Task<IActionResult> EditHotel(int id)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
            var hotelDto = new HotelDto()
            {
                Name = hotel.Name,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                Description = hotel.Description
            };

            return View(hotelDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditHotel(HotelDto hotelDto, int id)
        {
            if (ModelState.IsValid)
            {
                if (hotelDto.ImageFile == null && hotelDto.ImageFile?.Length < 0)
                {
                    ModelState.AddModelError("", "You must choose an image");
                    return View(hotelDto);
                }

                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(hotelDto.ImageFile!.FileName);

                var folderName = env.WebRootPath + "/Images/Hotels/";

                using (var stream = System.IO.File.Create(folderName + imageName))
                {
                    hotelDto.ImageFile.CopyTo(stream);
                }

                var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);

                hotel.Name = hotelDto.Name;
                hotel.Address = hotelDto.Address;
                hotel.City = hotelDto.City;
                hotel.Country = hotelDto.Country;
                hotel.PhoneNumber = hotelDto.PhoneNumber;
                hotel.Email = hotelDto.Email;
                hotel.Description = hotelDto.Description;
                hotel.ImageUrl = imageName;

                _unitOfWork.Hotels.Update(hotel);
                _unitOfWork.Save();

                return RedirectToAction("ShowHotels");
            }
            return View(hotelDto);
        }

        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _unitOfWork.Hotels.GetByIdAsync(id);
            var imagePath = env.WebRootPath + "/Images/Hotels/Hotels/" + hotel.ImageUrl;

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            
            _unitOfWork.Hotels.Delete(hotel);
            _unitOfWork.Save();
            return RedirectToAction("ShowHotels");
        }
		#endregion

		#region CRUD of Rooms
		public async Task<IActionResult> ShowRooms(int id)
        {
            var rooms = await _unitOfWork.Rooms.FindAllAsync(r => r.HotelId == id, new string[] { "Hotel" });
			TempData["HotelId"] = id;
			return View(rooms);
        }

        public IActionResult AddRoom(int id)
        {
			return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom(RoomDto roomDto)
        {
            if (ModelState.IsValid)
            {
                int hotelId = (int)TempData["HotelId"]!;
				var hotel = await _unitOfWork.Hotels.GetByIdAsync(hotelId);

				var imageName = Guid.NewGuid().ToString() + Path.GetExtension(roomDto.ImageUrl.FileName);
                var fileName = env.WebRootPath + "/Images/Rooms/";

                using (var stream = System.IO.File.Create(fileName + imageName))
                {
                    roomDto.ImageUrl.CopyTo(stream);
                }

                var room = new Room()
                {
                    RoomNumber = roomDto.RoomNumber,
                    Floor = roomDto.Floor,
                    Type = roomDto.Type,
                    BasePrice = roomDto.BasePrice,
                    AvailabilityStatus = "Available",
                    SpecialFeatures = roomDto.SpecialFeatures,
                    ImageUrl = imageName,
                    HotelId = hotel.Id,
                    Hotel = hotel
                };

                await _unitOfWork.Rooms.AddAsync(room);
                _unitOfWork.Save();

                return RedirectToAction("ShowRooms", new {id= hotelId });

            }
            return View(roomDto);
        }

        public IActionResult EditRoom(int id)
        {
            var room = _unitOfWork.Rooms.FindOneItem(r => r.Id == id, new string[] { "Hotel" });
            var roomDto = new RoomDto()
            {
                RoomNumber = room.RoomNumber,
                Floor = room.Floor,
                Type = room.Type,
                BasePrice = room.BasePrice,
                SpecialFeatures = room.SpecialFeatures,
            };
            return View(roomDto);
        }

        [HttpPost]
        public IActionResult EditRoom(RoomDto roomDto, int id)
        {
            if(ModelState.IsValid)
            {
				int hotelId = (int)TempData["HotelId"]!;

				var room = _unitOfWork.Rooms.FindOneItem(r => r.Id == id, new string[] { "Hotel" });

                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(roomDto.ImageUrl!.FileName);
                var fileName = env.WebRootPath + "/Images/Rooms/";

                using (var stream = System.IO.File.Create(fileName + imageName))
                {
                    roomDto.ImageUrl.CopyTo(stream);
                }

                room.RoomNumber=roomDto.RoomNumber;
                room.Floor=roomDto.Floor;
                room.Type=roomDto.Type;
                room.BasePrice=roomDto.BasePrice;
                room.SpecialFeatures=roomDto.SpecialFeatures;
                room.ImageUrl = imageName;

                _unitOfWork.Rooms.Update(room);
                _unitOfWork.Save();

				return RedirectToAction("ShowRooms", new { id = hotelId });
			}
            return View(roomDto);
        }
        #endregion

        #region of Bookings
        public async Task<IActionResult> ShowBookings()
        {
			var bookings = await _unitOfWork.Bookings.FindAllAsync(b => true, new string[] { "PaymentMethod", "User", "Room", "Room.Hotel" });
            return View(bookings);
		}

        public IActionResult DeleteBooking(int id)
        {
            return View();
        }

        [HttpPost]
        public IActionResult DeleteBooking(Booking booking, int id)
        {
            booking = _unitOfWork.Bookings.FindOneItem(b => b.Id == id, new string[] { "PaymentMethod", "User", "Room", "Room.Hotel" });
            booking.Room!.AvailabilityStatus = "Available";

            _unitOfWork.Bookings.Delete(booking);


            _unitOfWork.Save();

            return RedirectToAction("ShowBookings");
        }
        #endregion

        #region Users
        public async Task<IActionResult> ShowUsers()
        {
            int adminId = AccountController.GetUserId(HttpContext);
            var users = await _unitOfWork.Users.FindAllAsync(u => u.Id != adminId);
            return View(users);
        }
        #endregion
    }
}
