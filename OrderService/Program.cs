using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderService.Data.Context;
using OrderService.Data.Repositories.IRepository;
using OrderService.Data.Repositories.Repository;
using OrderService.Messaging;
using OrderService.Services.IService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Register repositories and services
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService.Services.Service.OrderService>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

// Configure RabbitMQ
//builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>(sp =>
//{
//    var configuration = sp.GetRequiredService<IConfiguration>();
//    var hostName = configuration["RabbitMQ:HostName"];
//    var userName = configuration["RabbitMQ:UserName"];
//    var password = configuration["RabbitMQ:Password"];
//    return new RabbitMQService(hostName, userName, password);
//});

// Register HttpClient for Product Service
builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductService"]);
});

// Configure JWT Authentication
builder.Services.AddDbContext<OrderDbContext>(options =>
options.UseSqlServer(
        builder.
        Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{

    var key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!);
    string issuer = builder.Configuration.GetSection("Jwt:Issuer").Value!;
    string audience = builder.Configuration.GetSection("Jwt:Audience").Value!;

    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero,
    };
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
