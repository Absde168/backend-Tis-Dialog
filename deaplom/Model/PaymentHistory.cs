namespace deaplom.Model
{
    public class PaymentHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = "Card";
        public string Status { get; set; } = "Success";
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();
    }
}