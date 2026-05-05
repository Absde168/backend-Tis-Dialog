namespace deaplom.Model
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty; // Уже есть у провайдера
        public string PasswordHash { get; set; } = string.Empty; // Хранится хэш
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        // Связи
        public List<ConnectionHistory> ConnectionHistory { get; set; } = new();
        public List<PaymentHistory> PaymentHistory { get; set; } = new();
        public List<ChatMessage> ChatMessages { get; set; } = new();
    }
}
