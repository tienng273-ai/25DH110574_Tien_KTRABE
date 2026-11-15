using System.Web.Mvc;

namespace _25DH110574_Tien_KTRABE.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            // Chưa đăng nhập
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Không phải Admin
            if (Session["UserRole"] == null ||
                Session["UserRole"].ToString() != "A")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}