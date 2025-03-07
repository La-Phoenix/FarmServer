using FarmServer;
using FarmServer.Infrastructure;
using FarmServer.Infrastructure.Repositories;
using FarmServer.Infrastructure.Services;
using FarmServer.Interfaces.IFarm;
using FarmServer.Interfaces.IFarmer;
using FarmServer.Interfaces.IField;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<FarmDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Register repositories
builder.Services.AddScoped<IFarmRepository, FarmRepository>();  // Repository for DB access
builder.Services.AddScoped<IFarmerRepository, FarmerRepository>();  // Repository for DB access
builder.Services.AddScoped<IFieldRepository, FieldRepository>();  // Repository for DB access


// Register services
builder.Services.AddScoped<IFarmService, FarmService>(); // Service for business logic`
builder.Services.AddScoped<IFarmerService, FarmerService>(); // Service for business logic
builder.Services.AddScoped<IFieldService, FieldService>(); // Service for business logic


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigration();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
