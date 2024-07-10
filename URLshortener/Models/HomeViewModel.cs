namespace URLshortener.Models
{
    public class HomeViewModel
    {
        public List<ShortUrl> ShortUrls { get; set; }
        public ShortUrl NewShortUrl { get; set; }
        public string OriginalUrlCode { get; set; }
    }
}
