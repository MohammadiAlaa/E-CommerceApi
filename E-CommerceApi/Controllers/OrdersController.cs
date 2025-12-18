using E_CommerceApi.DTOs;
using E_CommerceApi.Models;
using E_CommerceApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_CommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // يجب أن يكون مسجلاً
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _unitOfWork.Repository<Order>()
                .FindAllAsync(o => true, new[] { "Shipping", "Payment", "OrderItems" });
            return Ok(orders);
        }

        [HttpGet("MyOrders")]
        public async Task<IActionResult> GetOrdersHestory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _unitOfWork.Repository<Order>()
                .FindAllAsync(o => o.UserId == userId, new[] { "Shipping", "Payment" });
            return Ok(orders);
        }


        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto) // يفضل استخدام [FromBody] هنا
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // التأكد من وجود مستخدم (بالرغم من [Authorize] لكن للأمان)
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                OrderItems = new List<OrderItem>(),
                // إنشاء كائنات فرعية
                Shipping = new Shipping { Address = dto.Address, City = dto.City, ShippingMethod = "Standard" },
                Payment = new Payment { Method = dto.PaymentMethod, Status = "Pending", Amount = 0 }
            };

            decimal totalAmount = 0;

            foreach (var itemDto in dto.Items)
            {
                // يمكن تحسين هذه العملية باستخدام FindAllAsync مع Ids متعددة وتقليل طلبات قاعدة البيانات
                var product = await _unitOfWork.Repository<Item>().GetByIdAsync(itemDto.ItemId);

                if (product == null)
                    return BadRequest($"Item {itemDto.ItemId} not found.");

                if (product.Quantity < itemDto.Quantity)
                    return BadRequest($"Insufficient quantity for Item {product.Name}. Available: {product.Quantity}");

                // تحديث المخزون (يتم الحفظ في النهاية)
                product.Quantity -= itemDto.Quantity;
                _unitOfWork.Repository<Item>().Update(product); // وضع الكائن في حالة التحديث في الـ Context

                order.OrderItems.Add(new OrderItem
                {
                    ItemId = itemDto.ItemId,
                    Quantity = itemDto.Quantity,
                    Price = product.Price
                });

                totalAmount += product.Price * itemDto.Quantity;
            }

            order.Payment.Amount = totalAmount;

            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.CompleteAsync(); // حفظ جميع التغييرات (الطلب وتحديثات المخزون) مرة واحدة

            // 🚀 التعديل: إرجاع 201 CreatedAtAction
            return CreatedAtAction(nameof(Get), new { id = order.Id }, new { orderId = order.Id, message = "Order created successfully" });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var order = await _unitOfWork.Repository<Order>().FindAsync(
                o => o.Id == id && (isAdmin || o.UserId == userId),
                new[] { "Shipping", "Payment", "OrderItems" });

            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost("{id}/Cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _unitOfWork.Repository<Order>().FindAsync(
                o => o.Id == id && o.UserId == userId, 
                new[] { "OrderItems" });

            if (order == null) return NotFound("Order not found or access denied.");
            if (order.Status == "Cancelled" || order.Status == "Completed" || order.Status == "Shipped")
                return BadRequest("Cannot cancel this order as it is already processed or shipped.");

            // استعادة المخزون
            foreach (var orderItem in order.OrderItems)
            {
                var product = await _unitOfWork.Repository<Item>().GetByIdAsync(orderItem.ItemId);
                if (product != null)
                {
                    product.Quantity += orderItem.Quantity;
                    _unitOfWork.Repository<Item>().Update(product);
                }
            }

            order.Status = "Cancelled";
            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.CompleteAsync();
            return Ok(new { message = "Order cancelled and inventory restored." });
        }

        [HttpPut("{id}/Status")]
        [Authorize(Roles = "Admin,Driver")] 
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var order = await _unitOfWork.Repository<Order>().FindAsync(
                o => o.Id == id,
                new[] { "Shipping", "Payment" });

            if (order == null) return NotFound("Order not found.");

            order.Status = dto.Status;

            if (dto.Status == "Completed")
            {
                if (order.Shipping != null) order.Shipping.Status = "Delivered";
                if (order.Payment != null) order.Payment.Status = "Completed";
            }

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();

            return Ok(new { message = $"Order status updated to {dto.Status}" });
        }
    }
}