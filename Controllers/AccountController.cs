using System.Linq;
using System.Web.Mvc;
using _25DH110574_Tien_KTRABE.Models;

namespace _25DH110574_Tien_KTRABE.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // Hiển thị trang đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.";
                return View();
            }

            var user = db.Users.FirstOrDefault(u =>
                u.Username == username &&
                u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }

            var customer = db.Customers.FirstOrDefault(c =>
                c.Username == user.Username);

            if (customer == null)
            {
                ViewBag.Error = "Không tìm thấy thông tin khách hàng.";
                return View();
            }

            // Lưu thông tin đăng nhập vào Session
            Session["Username"] = user.Username;
            Session["CustomerID"] = customer.CustomerID;
            Session["CustomerName"] = customer.CustomerName;
            Session["UserRole"] = user.UserRole;

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        // Hiển thị trang đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // Xử lý đăng ký
        [HttpPost]
        public ActionResult Register(
            string username,
            string password,
            string customerName,
            string customerPhone,
            string customerEmail,
            string customerAddress)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(customerName) ||
                string.IsNullOrWhiteSpace(customerPhone))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ các thông tin bắt buộc.";
                return View();
            }

            // Kiểm tra Username đã tồn tại chưa
            var existingUser = db.Users
                .FirstOrDefault(u => u.Username == username);

            if (existingUser != null)
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại.";
                return View();
            }

            // Tạo User
            var user = new User
            {
                Username = username,
                Password = password,
                UserRole = "C"
            };

            db.Users.Add(user);
            db.SaveChanges();

            // Tạo Customer
            var customer = new Customer
            {
                CustomerName = customerName,
                CustomerPhone = customerPhone,
                CustomerEmail = customerEmail,
                CustomerAddress = customerAddress,
                Username = username
            };

            db.Customers.Add(customer);
            db.SaveChanges();

            return RedirectToAction("Login");
        }

        // Hiển thị trang chỉnh sửa tài khoản
        [HttpGet]
        public ActionResult Edit()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login");
            }

            string username = Session["Username"].ToString();

            var customer = db.Customers
                .FirstOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // Xử lý chỉnh sửa tài khoản
        [HttpPost]
        public ActionResult Edit(
            string customerName,
            string customerPhone,
            string customerEmail,
            string customerAddress)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("Login");
            }

            string username = Session["Username"].ToString();

            var customer = db.Customers
                .FirstOrDefault(c => c.Username == username);

            if (customer == null)
            {
                return HttpNotFound();
            }

            if (string.IsNullOrWhiteSpace(customerName) ||
                string.IsNullOrWhiteSpace(customerPhone))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ họ tên và số điện thoại.";
                return View(customer);
            }

            customer.CustomerName = customerName;
            customer.CustomerPhone = customerPhone;
            customer.CustomerEmail = customerEmail;
            customer.CustomerAddress = customerAddress;

            db.SaveChanges();

            Session["CustomerName"] = customer.CustomerName;

            ViewBag.Success = "Cập nhật thông tin thành công.";

            return View(customer);
        }
    }
}