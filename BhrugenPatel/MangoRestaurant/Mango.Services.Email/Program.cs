using Mango.Services.EmailApi.Contexts;
using Mango.Services.EmailApi.Extensions;
using Mango.Services.EmailApi.Messaging;
using Mango.Services.EmailApi.Models;
using Mango.Services.EmailApi.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<ServiceBusOptions>(
    builder.Configuration.GetSection("ServiceBus"));

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register our services
builder.Services.AddScoped<IEmailRepository, EmailRepository>();

var dbOptionBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(new EmailRepository(dbOptionBuilder.Options));
builder.Services.AddSingleton<IServiceBusConsumer, AzureServiceBusConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseAzureServiceBusConsumer();

app.Run();
