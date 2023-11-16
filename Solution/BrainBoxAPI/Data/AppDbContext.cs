using Microsoft.EntityFrameworkCore;
using BrainBoxAPI.Models;

namespace BrainBoxAPI.Data
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
        public DbSet<SolveRunModel> SolveRunModels { get; set; }
        public DbSet<QuestionSolveRunJoinModel> QuestionSolveRunJoinModels { get; set; }

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

            modelBuilder.Entity<SolveRunModel>()
                .HasOne(model => model.Room)
                .WithMany(model => model.SolveRuns)
                .HasForeignKey(model => model.RoomId)
                .IsRequired();

            modelBuilder.Entity<QuestionSolveRunJoinModel>()
                .HasOne(model => model.SelectedAnswerOption)
                .WithMany()
                .HasForeignKey(model => model.AnswerOptionModelID);

            modelBuilder.Entity<QuestionSolveRunJoinModel>()
                .HasOne(model => model.Question)
                .WithMany(model => model.Joins)
                .HasForeignKey(model => model.QuestionModelID);

            modelBuilder.Entity<QuestionSolveRunJoinModel>()
                .HasOne(model => model.SolveRun)
                .WithMany(model => model.SolveRunJoin)
                .HasForeignKey(model => model.SolveRunModelID);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "mydatabase.db");
            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}
