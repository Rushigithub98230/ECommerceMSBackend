using EComMSSharedLibrary.Extensions;
using Microsoft.EntityFrameworkCore;
using NotificationServiceNew.Data;
using NotificationServiceNew.Messaging;
using NotificationServiceNew.Repositories;
using NotificationServiceNew.Services;
using NotificationServiceNew.Services.EmailServices;
using NotificationServiceNew.Services.UserServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<NotificationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Repositories
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

// Services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHttpClient<IUserService, UserService>();
builder.Services.AddHostedService<RabbitMQConsumer>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseGlobalExceptionHandler();
app.UseAuthorization();

app.MapControllers();

app.Run();
