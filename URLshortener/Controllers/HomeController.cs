using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using URLshortener.Models;
using URLshortener.Services;
using System.Security.Claims;

namespace URLshortener.Controllers
{
    public class HomeController : Controller
    {
        private readonly ShortUrlService _shortUrlService;

        public HomeController(ShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        public IActionResult About()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                ShortUrls = await _shortUrlService.GetAllShortUrlsAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddShortUrl(HomeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var newUrl = new ShortUrl
                {
                    OriginalUrlCode = model.NewShortUrl.OriginalUrlCode,
                    CreatedById = userId,
                    CreatedDate = DateTime.UtcNow
                };

                try
                {
                    await _shortUrlService.CreateShortUrlAsync(newUrl);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            model.ShortUrls = await _shortUrlService.GetAllShortUrlsAsync();
            return View("Index", model);
        }
    }
}
