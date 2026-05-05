namespace deaplom.Model
{
    public class ConnectionHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime ConnectedAt { get; set; }
        public DateTime? DisconnectedAt { get; set; }

        public TimeSpan Duration => DisconnectedAt - ConnectedAt ?? TimeSpan.Zero;

        public long DataUsedBytes { get; set; }
    }
}