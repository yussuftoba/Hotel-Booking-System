using Microsoft.AspNetCore.Mvc;

namespace Hotel_Booking.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			string role = AccountController.GetUserRole(HttpContext);

			if (role == "Admin")
			{
				return RedirectToAction("Dashboard", "AdminRole");
			}
			else
			{
				return RedirectToAction("Index", "Hotel");
			}

		}
	}
}
