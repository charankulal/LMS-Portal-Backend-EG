namespace LMS.api.Models
{
    public class Announcement
    {
        public int BatchId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
