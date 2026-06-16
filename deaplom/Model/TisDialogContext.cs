using Microsoft.EntityFrameworkCore;

namespace deaplom.Model
{
    public class TisDialogContext : DbContext
    {
        public TisDialogContext(DbContextOptions<TisDialogContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ConnectionHistory> ConnectionHistory { get; set; }
        public DbSet<PaymentHistory> PaymentHistory { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<SystemNotification> SystemNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<ConnectionHistory>()
                .HasOne(ch => ch.User)
                .WithMany(u => u.ConnectionHistory)
                .HasForeignKey(ch => ch.UserId);

            modelBuilder.Entity<PaymentHistory>()
                .HasOne(ph => ph.User)
                .WithMany(u => u.PaymentHistory)
                .HasForeignKey(ph => ph.UserId);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(cm => cm.UserId);

            modelBuilder.Entity<SystemNotification>()
                .HasOne(sn => sn.User)
                .WithMany()
                .HasForeignKey(sn => sn.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
