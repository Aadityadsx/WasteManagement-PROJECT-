using System.Data.Entity;

namespace WasteManagementSystemMVC.Models
{
    public class WasteDbContext : DbContext
    {
        static WasteDbContext()
        {
            Database.SetInitializer<WasteDbContext>(null);
        }

        public WasteDbContext() : base("WasteDbContext")
        {
        }

        public DbSet<WasteComplaint> WasteComplaints { get; set; }

        public DbSet<Citizen> Citizens { get; set; }

        public DbSet<Worker> Workers { get; set; }

        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<CleaningProof> CleaningProofs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feedback>()
                .ToTable("Feedback");

            base.OnModelCreating(modelBuilder);
        }
    }

}