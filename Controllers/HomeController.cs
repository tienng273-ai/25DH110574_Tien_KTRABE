using System.Linq;
using System.Web.Mvc;
using _25DH110574_Tien_KTRABE.Models;

namespace _25DH110574_Tien_KTRABE.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        public ActionResult Index(string category, string search)
        {
            var products = db.Products  .Include("Category") .Where(p => p.Quantity > 0).AsQueryable();

            // Tìm kiếm sản phẩm theo tên
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(search));
            }

            // Lọc sản phẩm theo danh mục
            if (category == "tainghe")
            {
                products = products.Where(p =>
                    p.ProductName.Contains("Tai nghe") ||
                    p.ProductName.Contains("JBL"));
            }
            else if (category == "sacpin")
            {
                products = products.Where(p =>
                    p.ProductName.Contains("Sạc") ||
                    p.ProductName.Contains("Pin"));
            }
            else if (category == "capsac")
            {
                products = products.Where(p =>
                    p.ProductName.Contains("Dây sạc") ||
                    p.ProductName.Contains("Cáp"));
            }
            else if (category == "dienthoai")
            {
                products = products.Where(p =>
                    p.ProductName.Contains("iPhone") ||
                    (p.Category != null &&
                     p.Category.CategoryName.Contains("Điện thoại")));
            }

            ViewBag.Category = category;
            ViewBag.Search = search;

            return View(products.ToList());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your application contact page.";
            return View();
        }
    }
}