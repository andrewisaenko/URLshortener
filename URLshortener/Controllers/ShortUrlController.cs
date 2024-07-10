// ShortUrlController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using URLshortener.Data;
using URLshortener.Models;

namespace URLshortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShortUrlController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/ShortUrl/create

        [Authorize]
        [HttpPost("create")]
        public IActionResult CreateShortUrl([FromBody] ShortUrlCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Получаем текущего пользователя
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Создаем новый ShortUrl
            var shortUrl = new ShortUrl
            {
                OriginalUrlCode = model.OriginalUrlCode,
                ShortUrlCode = GenerateShortUrlCode(),
                CreatedById = userId, // Устанавливаем ID текущего пользователя
                CreatedDate = DateTime.UtcNow
            };

            _context.ShortUrls.Add(shortUrl);
            _context.SaveChanges();

            return Ok(new { shortUrl.ShortUrlCode });
        }

        // Example method to generate short URL code
        private string GenerateShortUrlCode()
        {
            // Implement your logic to generate short URL code
            // This is just an example
            return Guid.NewGuid().ToString().Substring(0, 6);
        }

        // GET: api/ShortUrl/{shortUrlCode}
        [HttpGet("{shortUrlCode}")]
        public IActionResult RedirectToOriginalUrl(string shortUrlCode)
        {
            var shortUrl = _context.ShortUrls.FirstOrDefault(u => u.ShortUrlCode == shortUrlCode);

            if (shortUrl == null)
            {
                return NotFound();
            }

            // Example of redirection
            // For a real application, you may want to do a permanent redirect (HTTP 301) instead
            return RedirectPermanent(shortUrl.OriginalUrlCode);
        }
    }

    // Data transfer object (DTO) for creating a short URL
    public class ShortUrlCreateDto
    {
        public string OriginalUrlCode { get; set; }
    }
}
