using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<QuestionModel> Questions { get; set; }
        public DbSet<AnswerOptionModel> AnswerOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomModel>()
                .HasMany(room => room.Questions)
                .WithOne(question => question.Room)
                .HasForeignKey(question => question.RoomId);

            modelBuilder.Entity<RoomModel>()
                .HasIndex(room => room.Name)
                .IsUnique();

            modelBuilder.Entity<QuestionModel>()
                .HasMany(question => question.AnswerOptions)
                .WithOne(option => option.Question)
                .HasForeignKey(option => option.QuestionId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "mydatabase.db");
            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}
