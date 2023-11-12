using API.Data;
using API.Managers;
using API.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ONE WAY TO DEFINE DB PATH
//var connectionString = builder.Configuration.GetConnectionString("SqLiteConnection");
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(connectionString));

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
// builder.Services.AddSingleton<IRepository<RoomModel>, RoomRepository>();

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
