using Hotel_Booking.ControllerBase;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Booking.Controllers
{
    public class RoomController : MVCControlBase
    {
        public RoomController(IUnitOfWork unitOfWork):base(unitOfWork)
        {
        }

        public async Task<IActionResult> Index(int id)
        {
            int userId = AccountController.GetUserId(HttpContext);
            ViewBag.UserId = userId;
            var rooms = await _unitOfWork.Rooms.FindAllAsync(r => r.HotelId == id, new string[] {"Hotel"});
            return View(rooms);
        }
    }
}
