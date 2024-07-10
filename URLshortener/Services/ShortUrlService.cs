using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using URLshortener.Data;
using URLshortener.Models;

namespace URLshortener.Services
{
    public class ShortUrlService
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;

        public ShortUrlService(AppDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task CreateShortUrlAsync(ShortUrl shortUrl)
        {
            if (await ShortUrlExistsAsync(shortUrl.OriginalUrlCode))
            {
                throw new InvalidOperationException("URL already exists.");
            }

            shortUrl.ShortUrlCode = GenerateShortCode(shortUrl.Id);
            shortUrl.CreatedDate = DateTime.UtcNow;
            shortUrl.CreatedById = _userService.GetCurrentUserId();
            _context.ShortUrls.Add(shortUrl);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShortUrl>> GetAllShortUrlsAsync()
        {
            return await _context.ShortUrls.ToListAsync();
        }

        public async Task<ShortUrl> GetShortUrlByIdAsync(int id)
        {
            return await _context.ShortUrls.FindAsync(id);
        }

        public async Task UpdateShortUrlAsync(ShortUrl updatedShortUrl)
        {
            var existingShortUrl = await _context.ShortUrls.FindAsync(updatedShortUrl.Id);
            if (existingShortUrl == null)
            {
                throw new InvalidOperationException("Short URL not found.");
            }

            existingShortUrl.OriginalUrlCode = updatedShortUrl.OriginalUrlCode;
            existingShortUrl.ShortUrlCode = updatedShortUrl.ShortUrlCode;
            existingShortUrl.CreatedById = _userService.GetCurrentUserId();
            existingShortUrl.CreatedDate = DateTime.UtcNow;

            _context.ShortUrls.Update(existingShortUrl);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteShortUrlAsync(int id)
        {
            var shortUrl = await _context.ShortUrls.FindAsync(id);
            if (shortUrl != null)
            {
                _context.ShortUrls.Remove(shortUrl);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ShortUrlExistsAsync(string originalUrl)
        {
            return await _context.ShortUrls.AnyAsync(u => u.OriginalUrlCode == originalUrl);
        }

        private string GenerateShortCode(int id)
        {
            const string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int baseLength = charset.Length;
            var result = new StringBuilder();

            while (id > 0)
            {
                result.Insert(0, charset[id % baseLength]);
                id /= baseLength;
            }

            // If the result is empty (id was 0), return the first character in the charset
            if (result.Length == 0)
            {
                result.Append(charset[0]);
            }

            return result.ToString();
        }
    }
}
