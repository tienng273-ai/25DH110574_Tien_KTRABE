using System;
using System.Linq;
using System.Web.Mvc;
using _25DH110574_Tien_KTRABE.Models;

namespace _25DH110574_Tien_KTRABE.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities1 db = new MyStoreEntities1();

        // GET: Orders
        public ActionResult Index(
            string customerName,
            string paymentStatus,
            DateTime? orderDate,
            int page = 1)
        {
            // Chỉ Admin được vào
            if (Session["Username"] == null ||
                Session["UserRole"] == null ||
                Session["UserRole"].ToString() != "A")
            {
                return RedirectToAction("Login", "Account");
            }

            int pageSize = 15;

            var orders = db.Orders
                .Include("Customer")
                .AsQueryable();

            // Tìm theo tên khách hàng
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                orders = orders.Where(o =>
                    o.Customer != null &&
                    o.Customer.CustomerName.Contains(customerName));
            }

            // Tìm theo trạng thái thanh toán
            if (!string.IsNullOrWhiteSpace(paymentStatus))
            {
                orders = orders.Where(o =>
                    o.PaymentStatus.Contains(paymentStatus));
            }

            // Tìm theo ngày đặt
            if (orderDate.HasValue)
            {
                DateTime fromDate = orderDate.Value.Date;
                DateTime toDate = fromDate.AddDays(1);

                orders = orders.Where(o =>
                    o.OrderDate >= fromDate &&
                    o.OrderDate < toDate);
            }

            int totalOrders = orders.Count();

            int totalPages =
                (int)Math.Ceiling((double)totalOrders / pageSize);

            if (page < 1)
                page = 1;

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            var result = orders
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CustomerName = customerName;
            ViewBag.PaymentStatus = paymentStatus;
            ViewBag.OrderDate = orderDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(result);
        }

        // Chi tiết đơn hàng
        public ActionResult DetailedOrder(int? id)
        {
            // Chỉ Admin được vào
            if (Session["Username"] == null ||
                Session["UserRole"] == null ||
                Session["UserRole"].ToString() != "A")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!id.HasValue)
            {
                return RedirectToAction("Index");
            }

            var order = db.Orders
                .Include("Customer")
                .Include("OrderDetails.Product")
                .FirstOrDefault(o => o.OrderID == id.Value);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // Cập nhật trạng thái
        [HttpPost]
        public ActionResult UpdateStatus(int id, string paymentStatus)
        {
            // Chỉ Admin được cập nhật
            if (Session["Username"] == null ||
                Session["UserRole"] == null ||
                Session["UserRole"].ToString() != "A")
            {
                return RedirectToAction("Login", "Account");
            }

            var order = db.Orders.Find(id);

            if (order == null)
            {
                return HttpNotFound();
            }

            order.PaymentStatus = paymentStatus;

            db.SaveChanges();

            return RedirectToAction(
                "DetailedOrder",
                new { id = id });
        }
    }
}