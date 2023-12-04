using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BrainBoxAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BrainBoxAPI.Data
{

    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Rooms = new List<RoomModel>();
            Quizzes = new List<QuizModel>();
        }

        public ICollection<RoomModel> Rooms { get; set; }
        public ICollection<QuizModel> Quizzes { get; set; }
    }


    public class AppDbContext : IdentityUserContext<ApplicationUser>
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<QuestionModel> Questions { get; set; }
        public DbSet<AnswerOptionModel> AnswerOptions { get; set; }
        public DbSet<QuizModel> QuizModels { get; set; }
        public DbSet<QuizQuestionRelationModel> QuizQuestionRelationModels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


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

            modelBuilder.Entity<QuizModel>()
                .HasOne(model => model.Room)
                .WithMany(model => model.Quizs)
                .HasForeignKey(model => model.RoomId)
                .IsRequired();

            modelBuilder.Entity<QuizQuestionRelationModel>()
                .HasOne(model => model.SelectedAnswerOption)
                .WithMany()
                .HasForeignKey(model => model.AnswerOptionModelID);

            modelBuilder.Entity<QuizQuestionRelationModel>()
                .HasOne(model => model.Question)
                .WithMany(model => model.QuizRelations)
                .HasForeignKey(model => model.QuestionModelID);

            modelBuilder.Entity<QuizQuestionRelationModel>()
                .HasOne(model => model.Quiz)
                .WithMany(model => model.QuestionRelations)
                .HasForeignKey(model => model.QuizModelID);

            modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Rooms)
            .WithMany(r => r.Users)
            .UsingEntity(j => j.ToTable("UserRooms"));

            modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Quizzes)
            .WithOne(q => q.User)
            .HasForeignKey(q => q.UserId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "mydatabase.db");
            options.UseSqlite($"Data Source={dbPath}");
        }

        public void SeedTestData()
        {
            if (!Rooms.Any())
            {
                var rooms = new List<RoomModel>
                {
                    new RoomModel { Name = "Room 1" },
                    new RoomModel { Name = "Room 2" },
                };

                Rooms.AddRange(rooms);
                SaveChanges();

                if (!Questions.Any())
                {
                    var room1 = Rooms.First();
                    var questions = new List<QuestionModel>
                    {
                        new QuestionModel { Title = "Question 1", RoomId = room1.Id },
                        new QuestionModel { Title = "Question 2", RoomId = room1.Id },
                    };

                    Questions.AddRange(questions);
                    SaveChanges();
                }
            }
        }
    }
}
