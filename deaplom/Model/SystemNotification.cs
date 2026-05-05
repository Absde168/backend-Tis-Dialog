namespace deaplom.Model
{
    public class SystemNotification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = "Info"; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        // Опционально: привязка к пользователю, если уведомления персонализированы
        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
