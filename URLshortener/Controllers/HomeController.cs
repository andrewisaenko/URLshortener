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
    }
}
