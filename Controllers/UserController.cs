using System.Linq;
using System.Web.Mvc;
using _25DH110574_Tien_KTRABE.Models;

namespace _25DH110574_Tien_KTRABE.Controllers
{
    public class UserController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // Danh sách User/Admin
        public ActionResult Index()
        {
            var users = db.Users
                .OrderBy(u => u.Username)
                .ToList();

            return View(users);
        }

        // Thêm User
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (db.Users.Any(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        // Sửa User
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(user);
        }

        // Xóa User
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            if (user.Customers.Any())
            {
                TempData["Error"] = "Không thể xóa User vì đang có khách hàng sử dụng tài khoản này.";
                return RedirectToAction("Index");
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}