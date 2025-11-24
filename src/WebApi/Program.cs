using Application.UseCases;
using Domain.Interfaces;
using Domain.Services;
using Infrastructure.Data;
using Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Clean Architecture Orders API", 
        Version = "v1.0",
        Description = "Orders API following Clean Architecture principles"
    });
});

// Register dependencies following Dependency Injection pattern
// Singleton for logger (stateless)
builder.Services.AddSingleton<ILogger>(sp => new ConsoleLogger(true));

// Scoped for services that maintain state per request
builder.Services.AddScoped<IOrderService, OrderService>();

// Scoped for repository (maintains connection per request)
builder.Services.AddScoped<IOrderRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger>();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=localhost;Database=OrdersDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True";
    return new OrderRepository(connectionString, logger);
});

// Scoped for use cases
builder.Services.AddScoped<CreateOrderUseCase>();

// Configure CORS properly (not allowing everything)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173") // Add your frontend URLs
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use proper error handling middleware
app.UseExceptionHandler("/error");

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthorization();
app.MapControllers();

// Global error handler endpoint
app.Map("/error", (HttpContext context) =>
{
    return Results.Problem(
        title: "An error occurred",
        statusCode: StatusCodes.Status500InternalServerError
    );
});

app.Run();
