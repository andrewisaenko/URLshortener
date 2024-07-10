namespace URLshortener.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }
        public string OriginalUrlCode { get; set; }
        public string ShortUrlCode { get; set; }
        public int CreatedById { get; set; } 
        public User CreatedBy { get; set; } 
        public DateTime CreatedDate { get; set; }
    }
}
