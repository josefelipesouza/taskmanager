using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Commands;
using TaskManager.Application.Validators;
using TaskManager.Domain.Interfaces;
using TaskManager.Application.Interfaces;
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

// RabbitMQ
builder.Services.AddSingleton<IMessageService>(sp =>
    new RabbitMqService(builder.Configuration["RabbitMQ:Host"] ?? "localhost"));

var app = builder.Build();

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