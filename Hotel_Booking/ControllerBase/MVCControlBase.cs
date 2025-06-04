using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Booking.ControllerBase
{
    public class MVCControlBase:Controller
    {
        protected IUnitOfWork _unitOfWork;

        public MVCControlBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
