using _25DH110574_Tien_KTRABE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace _25DH110574_Tien_KTRABE.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = Session["Cart"] as Dictionary<int, int>;

            if (cart == null)
            {
                cart = new Dictionary<int, int>();
            }

            var productIds = cart.Keys.ToList();

            var products = db.Products
                .Where(p => productIds.Contains(p.ProductID))
                .ToList();

            ViewBag.Cart = cart;

            return View(products);
        }

        // Tăng số lượng
        public ActionResult Add(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);

            if (product == null)
                return HttpNotFound();

            if (product.Quantity <= 0)
            {
                TempData["CartMessage"] = "Sản phẩm đã hết hàng.";
                return RedirectToAction("Index", "Home");
            }

            var cart = Session["Cart"] as Dictionary<int, int>;

            if (cart == null)
                cart = new Dictionary<int, int>();

            if (!cart.ContainsKey(id))
            {
                cart[id] = 1;
            }
            else if (cart[id] < product.Quantity)
            {
                cart[id]++;
            }
            else
            {
                TempData["CartMessage"] = "Số lượng sản phẩm đã đạt tối đa tồn kho.";
            }

            Session["Cart"] = cart;

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Increase(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            var cart = Session["Cart"] as Dictionary<int, int>;

            if (cart == null)
            {
                cart = new Dictionary<int, int>();
            }

            if (!cart.ContainsKey(id))
            {
                cart[id] = 1;
            }
            else if (cart[id] < product.Quantity)
            {
                cart[id]++;
            }
            else
            {
                TempData["CartMessage"] = "Số lượng sản phẩm đã đạt tối đa tồn kho.";
            }

            Session["Cart"] = cart;

            return RedirectToAction("Index");
        }

        // Giảm số lượng
        public ActionResult Decrease(int id)
        {
            var cart = Session["Cart"] as Dictionary<int, int>;

            if (cart != null && cart.ContainsKey(id))
            {
                if (cart[id] > 1)
                {
                    cart[id]--;
                }
            }

            Session["Cart"] = cart;

            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ
        public ActionResult Remove(int id)
        {
            var cart = Session["Cart"] as Dictionary<int, int>;

            if (cart != null && cart.ContainsKey(id))
            {
                cart.Remove(id);
            }

            Session["Cart"] = cart;

            return RedirectToAction("Index");
        }

        // Tiếp tục mua hàng
        public ActionResult ContinueShopping()
        {
            return RedirectToAction("Index", "Home");
        }

        // Thanh toán
        public ActionResult Checkout()
        {
            var cart = Session["Cart"] as Dictionary<int, int>;

            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index");
            }

            if (Session["CustomerID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int customerId = (int)Session["CustomerID"];
            var customer = db.Customers.FirstOrDefault(c => c.CustomerID == customerId);
            if (customer == null)
            {
                return HttpNotFound();
            }



            var productIds = cart.Keys.ToList();

            var products = db.Products
                .Where(p => productIds.Contains(p.ProductID))
                .ToList();

            // Kiểm tra tồn kho
            foreach (var item in cart)
            {
                var product = products.FirstOrDefault(p => p.ProductID == item.Key);

                if (product == null)
                {
                    return HttpNotFound();
                }

                if (item.Value > product.Quantity)
                {
                    TempData["CartMessage"] =
                        "Sản phẩm " + product.ProductName +
                        " không đủ số lượng trong kho.";

                    return RedirectToAction("Index");
                }
            }

            // Tạo Order
            var order = new Order
            {
                CustomerID = customerId,
                OrderDate = DateTime.Now,
                PaymentStatus = "Chờ thanh toán",
                AddressDelivery = customer.CustomerAddress,
                TotalAmount = 0
            };

            db.Orders.Add(order);
            db.SaveChanges();

            decimal total = 0;

            // Tạo OrderDetail + trừ tồn kho
            foreach (var item in cart)
            {
                var product = products.First(p => p.ProductID == item.Key);

                int quantity = item.Value;

                var detail = new OrderDetail
                {
                    OrderID = order.OrderID,
                    ProductID = product.ProductID,
                    Quantity = quantity,
                    UnitPrice = product.ProductPrice
                };

                db.OrderDetails.Add(detail);

                product.Quantity -= quantity;

                total += product.ProductPrice * quantity;
            }

            order.TotalAmount = total;

            db.SaveChanges();

            // Xóa giỏ hàng
            Session["Cart"] = null;

            return RedirectToAction("MyOrder", "Customer");
        }
    }
} 