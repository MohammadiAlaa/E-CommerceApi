using E_CommerceApi.DTOs;
using E_CommerceApi.Models;
using E_CommerceApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Driver")]
    public class ShippingsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShippingsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPatch("{id}/Status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var shipping = await _unitOfWork.Repository<Shipping>().GetByIdAsync(id);
            if (shipping == null) return NotFound("Shipping record not found.");

            shipping.Status = dto.Status;
            shipping.LastUpdated = DateTime.Now;
            _unitOfWork.Repository<Shipping>().Update(shipping);

            if (dto.Status == "Delivered")
            {
                var order = await _unitOfWork.Repository<Order>().GetByIdAsync(shipping.OrderId);

                if (order != null)
                {
                    order.Status = "Completed";
                    _unitOfWork.Repository<Order>().Update(order);
                }
            }
            else if (dto.Status == "Shipped")
            {
                var order = await _unitOfWork.Repository<Order>().GetByIdAsync(shipping.OrderId);
                if (order != null)
                {
                    order.Status = "Shipped"; 
                    _unitOfWork.Repository<Order>().Update(order);
                }
            }

            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}