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
        public ActionResult Index(string customerName, string paymentStatus, DateTime? orderDate, int page = 1)
        {
            int pageSize = 15;

            var orders = db.Orders.Include("Customer").AsQueryable();

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

            // Tổng số đơn sau khi tìm kiếm
            int totalOrders = orders.Count();

            // Tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            // Đảm bảo page không vượt quá số trang
            if (page < 1)
                page = 1;

            if (totalPages > 0 && page > totalPages)
                page = totalPages;

            // Lấy 15 đơn của trang hiện tại
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
        public ActionResult DetailedOrder(int? id)
        {
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

        [HttpPost]
        public ActionResult UpdateStatus(int id, string paymentStatus)
        {
            var order = db.Orders.Find(id);

            if (order == null)
            {
                return HttpNotFound();
            }

            order.PaymentStatus = paymentStatus;

            db.SaveChanges();

            return RedirectToAction("DetailedOrder", new { id = id });
        }
    }
}