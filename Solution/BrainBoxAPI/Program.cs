using BrainBoxAPI.Caching;
using BrainBoxAPI.Data;
using BrainBoxAPI.Managers;
using BrainBoxAPI.Models;
using Microsoft.EntityFrameworkCore;
using SharedModels.Lobby;

namespace BrainBoxAPI;

public class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddLogging(loggingBuilder => {
            loggingBuilder.AddConsole()
                .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information);
            loggingBuilder.AddDebug();
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.EnableSensitiveDataLogging(true);
        });


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowAnyOrigin();
            });
        });
        builder.Services.AddAutoMapper(m =>
        {
            m.AddProfile(new AutoMappingProfile());
        });


        builder.Services.AddScoped<IRepository<QuestionModel>, QuestionRepository>();
        builder.Services.AddScoped<IRepository<RoomModel>, RoomRepository>();
        builder.Services.AddScoped<IQuizQuestionRelationRepository, QuizQuestionRelationRepository>();
        builder.Services.AddSingleton<IDictionaryCache<int, RoomContentDTO>, RoomContentCache>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
