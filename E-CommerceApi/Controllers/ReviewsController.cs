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
    public class ReviewsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasPurchased = await _unitOfWork.Repository<Order>().FindAsync(o =>
                o.UserId == userId &&
                o.Status == "Completed" &&
                o.OrderItems.Any(oi => oi.ItemId == dto.ItemId),
                new[] { "OrderItems" }
            );

            
            var review = new Review
            {
                ItemId = dto.ItemId,
                UserId = userId,
                Comment = dto.Comment,
                Rating = dto.Rating,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.Repository<Review>().AddAsync(review);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetItemReviews), new { itemId = review.ItemId }, review);
        }

        [HttpGet("Item/{itemId}")]
        public async Task<IActionResult> GetItemReviews(int itemId)
        {
            var reviews = await _unitOfWork.Repository<Review>().FindAllAsync(r => r.ItemId == itemId);
            return Ok(reviews);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var review = await _unitOfWork.Repository<Review>().GetByIdAsync(id);

            if (review == null) return NotFound();

            if (review.UserId != userId && !isAdmin)
            {
                return Forbid("You can only delete your own reviews.");
            }

            _unitOfWork.Repository<Review>().Delete(review);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}