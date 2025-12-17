using E_CommerceApi.DTOs;
using E_CommerceApi.Models;
using E_CommerceApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_CommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public ItemsController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _unitOfWork.Repository<Item>().GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _unitOfWork.Repository<Item>().GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromForm] CreateItemDto itemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            string? imageUrl = null;

            if (itemDto.ImageFile != null)
            {
                // 1. منطق حفظ الملف
                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + itemDto.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await itemDto.ImageFile.CopyToAsync(fileStream);
                }
                imageUrl = $"/images/{uniqueFileName}";
            }

            // 2. إنشاء الكائن من الـ DTO
            var item = new Item
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                Quantity = itemDto.Quantity,
                ImageUrl = imageUrl, // تعيين المسار المُنشأ هنا
                CategoryId = itemDto.CategoryId
            };

            await _unitOfWork.Repository<Item>().AddAsync(item);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateItemDto itemDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingItem = await _unitOfWork.Repository<Item>().GetByIdAsync(id);

            if (existingItem == null) return NotFound($"Item with ID {id} not found.");

            if (itemDto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(existingItem.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath, existingItem.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + itemDto.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await itemDto.ImageFile.CopyToAsync(fileStream);
                }
                existingItem.ImageUrl = $"/images/{uniqueFileName}";
            }

            

            if (!string.IsNullOrWhiteSpace(itemDto.Name))
                existingItem.Name = itemDto.Name;

            if (!string.IsNullOrWhiteSpace(itemDto.Description))
                existingItem.Description = itemDto.Description;

            if (itemDto.Price.HasValue)
                existingItem.Price = itemDto.Price.Value;

            if (itemDto.Quantity.HasValue)
                existingItem.Quantity = itemDto.Quantity.Value;

            if (itemDto.CategoryId.HasValue)
                existingItem.CategoryId = itemDto.CategoryId.Value;

            _unitOfWork.Repository<Item>().Update(existingItem);
            await _unitOfWork.CompleteAsync();

            return Ok(existingItem);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.Repository<Item>().GetByIdAsync(id);
            if (item == null) return NotFound();

            // إضافة منطق لحذف الصورة من السيرفر قبل حذف الكائن من قاعدة البيانات (للحفاظ على مساحة التخزين)
            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, item.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _unitOfWork.Repository<Item>().Delete(item);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}