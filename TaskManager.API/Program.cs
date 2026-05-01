using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TaskManager.Application.Commands;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Validators;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Messaging;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(CreateTaskCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskValidator>();

// Entity Framework + PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositório
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// RabbitMQ Producer
builder.Services.AddSingleton<IMessageService>(sp =>
    new RabbitMqService(builder.Configuration["RabbitMQ:Host"] ?? "localhost"));

// Email Service
builder.Services.AddScoped<IEmailService>(sp => new EmailService(
    builder.Configuration["Email:Host"] ?? "smtp.gmail.com",
    int.Parse(builder.Configuration["Email:Port"] ?? "587"),
    builder.Configuration["Email:Username"] ?? "",
    builder.Configuration["Email:Password"] ?? "",
    builder.Configuration["Email:From"] ?? ""
));

// RabbitMQ Consumer (Background Service)
builder.Services.AddSingleton<IHostedService>(sp => new RabbitMqConsumer(
    sp,
    sp.GetRequiredService<ILogger<RabbitMqConsumer>>(),
    builder.Configuration["RabbitMQ:Host"] ?? "localhost",
    builder.Configuration["Email:To"] ?? ""
));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:5174",
            "http://localhost:3000"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Aplicar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();