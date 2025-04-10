using EComMSSharedLibrary.Extensions;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderService.Data.DataAccessRepositories;
using OrderService.Services;
using OrderService.Validators;
using System.Text;
using OrderService.Services.ProductService;
using OrderService.Data;
using OrderService.Services.NotificationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddScoped<IOrderRepository, OrderRepository>();


builder.Services.AddScoped<IOrderService, OrderService.Services.OrderService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddScoped<INotificationService, NotificationService>();


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderDtoValidator>();


builder.Services.AddDbContext<OrderDbContext>(options =>
options.UseSqlServer(
        builder.
        Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddJwtAuthentication(builder.Configuration);


    


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
