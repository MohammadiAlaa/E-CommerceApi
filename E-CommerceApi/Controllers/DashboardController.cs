using E_CommerceApi.Models;
using E_CommerceApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Stats")]
        public async Task<IActionResult> GetStats()
        {
            var completedOrders = await _unitOfWork.Repository<Order>()
                .FindAllAsync(o => o.Status == "Completed", new[] { "Payment" });

            var totalRevenue = completedOrders.Sum(o => o.Payment?.Amount ?? 0);

            var lowStockItems = await _unitOfWork.Repository<Item>()
                .FindAllAsync(i => i.Quantity < 5);

            var totalOrdersCount = (await _unitOfWork.Repository<Order>().GetAllAsync()).Count();


            return Ok(new
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrdersCount,
                LowStockCount = lowStockItems.Count(),
                LowStockItems = lowStockItems.Select(i => new { i.Name, i.Quantity })
            });
        }

        [HttpGet("TopSellingProducts")]
        public async Task<IActionResult> GetTopSelling()
        {
            var orderItems = await _unitOfWork.Repository<OrderItem>()
                .FindAllAsync(oi => true, new[] { "Item" });

            var topProducts = orderItems
                .GroupBy(oi => oi.ItemId)
                .Select(g => new
                {
                    ProductName = g.First().Item.Name,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(5); 

            return Ok(topProducts);
        }
    }
}