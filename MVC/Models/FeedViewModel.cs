namespace MVC.Models
{
    public class FeedViewModel
    {
        public int id { get; set; }
        public string date { get; set; }
        public string postedBy { get; set; }
        public string content {  get; set; }
        public byte[]? ProfilePhoto {  get; set; }
    }
}
