using System.Linq;
using System.Web.Mvc;
using _25DH110574_Tien_KTRABE.Models;

namespace _25DH110574_Tien_KTRABE.Controllers
{
    public class ProductController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // GET: Product
        public ActionResult Index()
        {
            var products = db.Products
                .Include("Category")
                .ToList();

            return View(products);
        }
        // GET: Product/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(
                db.Categories,
                "CategoryID",
                "CategoryName",
                product.CategoryID
            );

            return View(product);
        }

        // GET: Product/Delete
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra sản phẩm đã có trong đơn hàng chưa
            bool hasOrderDetail = db.OrderDetails.Any(od => od.ProductID == id);

            if (hasOrderDetail)
            {
                TempData["Error"] = "Không thể xóa sản phẩm vì sản phẩm đã có trong đơn hàng.";
                return RedirectToAction("Index");
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Product/Edit
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryID = new SelectList(
                db.Categories,
                "CategoryID",
                "CategoryName",
                product.CategoryID
            );

            return View(product);
        }

        // POST: Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State =
                    System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(
                db.Categories,
                "CategoryID",
                "CategoryName",
                product.CategoryID
            );

            return View(product);
        }
        public ActionResult Details(int id)
        {
            var product = db.Products
                .Include("Category")
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        public ActionResult Inventory()
        {
            var products = db.Products
                .Include("Category")
                .OrderBy(p => p.Quantity)
                .ToList();

            return View(products);
        }
    }
}