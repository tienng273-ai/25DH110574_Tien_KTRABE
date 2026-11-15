using System.Linq;
using System.Web.Mvc;
using _25DH110574_Tien_KTRABE.Models;

namespace _25DH110574_Tien_KTRABE.Controllers
{
    public class CategoryController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // Danh sách
        public ActionResult Index()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        // Thêm - GET
        public ActionResult Create()
        {
            return View();
        }

        // Thêm - POST
        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                ModelState.AddModelError(
                    "CategoryName",
                    "Vui lòng nhập tên danh mục.");
            }

            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        // Sửa - GET
        public ActionResult Edit(int id)
        {
            var category = db.Categories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            return View(category);
        }

        // Sửa - POST
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                ModelState.AddModelError(
                    "CategoryName",
                    "Vui lòng nhập tên danh mục.");
            }

            if (ModelState.IsValid)
            {
                db.Entry(category).State =
                    System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(category);
        }

        // Xóa
        public ActionResult Delete(int id)
        {
            var category = db.Categories.Find(id);

            if (category == null)
            {
                return HttpNotFound();
            }

            if (category.Products.Any())
            {
                TempData["Error"] =
                    "Không thể xóa danh mục vì đang có sản phẩm.";
                return RedirectToAction("Index");
            }

            db.Categories.Remove(category);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}