// ShortUrlController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
         
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
           
            var shortUrl = new ShortUrl
            {
                OriginalUrlCode = model.OriginalUrlCode,
                ShortUrlCode = GenerateShortUrlCode(userId),
                CreatedById = userId, 
                CreatedDate = DateTime.UtcNow
            };

            _context.ShortUrls.Add(shortUrl);
            _context.SaveChanges();

            return Ok(new { shortUrl.ShortUrlCode });
        }
                
        private string GenerateShortUrlCode(int id)
        {
            const string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int baseLength = charset.Length;
            var result = new StringBuilder();

            while (id > 0)
            {
                result.Insert(0, charset[id % baseLength]);
                id /= baseLength;
            }
         
            if (result.Length == 0)
            {
                result.Append(charset[0]);
            }

            return result.ToString();
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
           
            return RedirectPermanent(shortUrl.OriginalUrlCode);
        }
    }

    public class ShortUrlCreateDto
    {
        public string OriginalUrlCode { get; set; }
    }
}
