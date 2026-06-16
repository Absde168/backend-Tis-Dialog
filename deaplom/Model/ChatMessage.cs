using System.Text.Json.Serialization;

namespace deaplom.Model
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [JsonIgnore]
        public User? User { get; set; }

        public string Content { get; set; } = string.Empty;
        public string SenderType { get; set; } = "User";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
