using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;

namespace Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Subject).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.IpAddress).HasMaxLength(45);
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.CreatedAt);
            });
        }

        public static void ConfigureSerilogTable(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEvent>(entity =>
            {
                entity.ToTable("logs");
                entity.HasKey("Id");
                entity.Property<int>("Id").ValueGeneratedOnAdd();
                entity.Property<string>("message");
                entity.Property<string>("message_template");
                entity.Property<string>("level").HasMaxLength(50);
                entity.Property<DateTimeOffset>("timestamp");
                entity.Property<string>("exception");
                entity.Property<string>("log_event");
            });
        }
    }
}
