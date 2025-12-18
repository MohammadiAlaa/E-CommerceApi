using E_CommerceApi.Models;
using E_CommerceApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // تم إضافته

namespace E_CommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PaymentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("{id}/Confirm")]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var payment = await _unitOfWork.Repository<Payment>().FindAsync(p => p.Id == id, new[] { "Order" });

            if (payment == null) return NotFound("Payment record not found.");

            if (payment.Order.UserId != userId && !isAdmin)
                return Forbid();

            if (payment.Status == "Completed") return BadRequest("Payment is already confirmed.");

            payment.Status = "Completed";
            payment.PaymentDate = DateTime.Now;
            _unitOfWork.Repository<Payment>().Update(payment);

            if (payment.Order != null && payment.Order.Status == "Pending")
            {
                payment.Order.Status = "Processing";
                _unitOfWork.Repository<Order>().Update(payment.Order);
            }

            await _unitOfWork.CompleteAsync();
            return Ok(new { message = "Payment confirmed and Order is now Processing." });
        }
    }
}