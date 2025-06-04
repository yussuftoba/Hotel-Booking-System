using Hotel_Booking.ControllerBase;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Booking.Controllers
{
    public class HotelController : MVCControlBase
    {
        public HotelController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IActionResult> Index()
        {
            var Hotels = await _unitOfWork.Hotels.GetAllAsync();
            return View(Hotels);
        }
    }
}
