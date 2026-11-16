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

            db.Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}