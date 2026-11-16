using _25DH110574_Tien_KTRABE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace _25DH110574_Tien_KTRABE.Controllers
{
    public class CustomerController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        public ActionResult TransactionHistory()
        {
            if (Session["CustomerID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int customerId = (int)Session["CustomerID"];

            var orders = db.Orders
                .Where(o => o.CustomerID == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }
        public ActionResult Index()
        {
            var customers = db.Customers
                .OrderBy(c => c.CustomerID)
                .ToList();

            return View(customers);
        }

        public ActionResult OrderDetail(int id)
        {
            if (Session["CustomerID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int customerId = (int)Session["CustomerID"];

            var order = db.Orders
                .Include("Customer")
                .Include("OrderDetails.Product")
                .FirstOrDefault(o =>
                    o.OrderID == id &&
                    o.CustomerID == customerId);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        public ActionResult Details(int id)
        {
            var customer = db.Customers
                .Include("Orders.OrderDetails.Product")
                .FirstOrDefault(c => c.CustomerID == id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        public ActionResult MyOrder(string search, string status, bool xemThem = false)
        {
            if (Session["CustomerID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int customerId = (int)Session["CustomerID"];


            var orders = db.Orders
                .Include("OrderDetails.Product")
                .Where(o => o.CustomerID == customerId)
                .AsQueryable();

            // Tìm kiếm theo mã đơn hàng hoặc tên sản phẩm
            if (!string.IsNullOrWhiteSpace(search))
            {
                int orderId;

                if (int.TryParse(search, out orderId))
                {
                    orders = orders.Where(o =>
                        o.OrderID == orderId ||
                        o.OrderDetails.Any(d =>
                            d.Product.ProductName.Contains(search)));
                }
                else
                {
                    orders = orders.Where(o =>
                        o.OrderDetails.Any(d =>
                            d.Product.ProductName.Contains(search)));
                }
            }

            // Lọc theo trạng thái nếu đang chọn một tab cụ thể
            if (!string.IsNullOrWhiteSpace(status))
            {
                string statusFilter = status.Trim();

                orders = orders.Where(o =>
                    o.PaymentStatus != null &&
                    o.PaymentStatus.Trim() == statusFilter);
            }

            orders = orders.OrderByDescending(o => o.OrderDate);

            var allOrders = orders.ToList();

            // Nếu chọn một trạng thái:
            // 2 đơn gần nhất hoặc tối đa 50 đơn khi Xem thêm
            // Nếu chọn Tất cả:
            // lấy tối đa 2 đơn gần nhất của mỗi trạng thái
            if (string.IsNullOrWhiteSpace(status))
            {
                if (!xemThem)
                {
                    allOrders = allOrders
                        .GroupBy(o => o.PaymentStatus)
                        .SelectMany(g => g
                            .OrderByDescending(o => o.OrderDate)
                            .Take(2))
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();
                }
                else
                {
                    allOrders = allOrders
                        .GroupBy(o => o.PaymentStatus)
                        .SelectMany(g => g
                            .OrderByDescending(o => o.OrderDate)
                            .Take(50))
                        .OrderByDescending(o => o.OrderDate)
                        .ToList();
                }
            }
            else
            {
                if (!xemThem)
                {
                    allOrders = allOrders
                        .Take(2)
                        .ToList();
                }
                else
                {
                    allOrders = allOrders
                        .Take(50)
                        .ToList();
                }
            }

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.CustomerId = customerId;
            ViewBag.XemThem = xemThem;

            // Kiểm tra còn đơn hàng để hiển thị Xem thêm
            if (string.IsNullOrWhiteSpace(status))
            {
                ViewBag.HasMore = orders
                    .ToList()
                    .GroupBy(o => o.PaymentStatus)
                    .Any(g => g.Count() > (xemThem ? 50 : 2));
            }
            else
            {
                ViewBag.HasMore = orders.Count() > (xemThem ? 50 : 2);
            }

            return View(allOrders);
        }
        public ActionResult ProductDetail(int id)
        {
            var product = db.Products
                .Include("Category")
                .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpPost]
        public ActionResult BuyAgain(int id, int quantity)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            if (quantity <= 0)
            {
                quantity = 1;
            }

            if (quantity > product.Quantity)
            {
                TempData["Error"] = "Số lượng mua vượt quá số lượng tồn kho.";
                return RedirectToAction("ProductDetail", new { id = id });
            }

            var cart = Session["Cart"] as Dictionary<int, int>;

            if (cart == null)
            {
                cart = new Dictionary<int, int>();
            }

            cart[id] = quantity;

            Session["Cart"] = cart;

            return RedirectToAction("Index", "Cart");
        }
    }
}