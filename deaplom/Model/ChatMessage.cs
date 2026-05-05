namespace deaplom.Model
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Content { get; set; } = string.Empty;
        public string SenderType { get; set; } = "User"; 
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
