using FarmServer;
using FarmServer.Infrastructure;
using FarmServer.Infrastructure.Repositories;
using FarmServer.Infrastructure.Services;
using FarmServer.Interfaces.IFarm;
using FarmServer.Interfaces.IFarmer;
using FarmServer.Interfaces.IField;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


//Load environment-specific configurations
var env = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

//Load JWT configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Get secret key dynamically (from env variable if not in config)
var secret = builder.Configuration["JwtSettings:Secret"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new InvalidOperationException("JWT Secret is missing.");

var key = Encoding.UTF8.GetBytes(secret);

//Registers JWT authentication as the default authentication method.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,         // Ensure token comes from a trusted issuer
            ValidateAudience = false,      //  Ignore audience validation
            ValidateLifetime = true,       // Ensure token has not expired
            ValidateIssuerSigningKey = true, // Ensure token is signed by a trusted key
            // Configure Expected Token Values
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            //ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

//Register authorization services in the DI(Dependency Injection) container
builder.Services.AddAuthorization();

//Registers MVC controllers (for API endpoints) in the DI container.
// This tells the serializer to handle circular references instead of throwing an error
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    { 
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Add a JWT Authorization header in Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Farm API", Version = "v1" });

    // Add JWT Authorization Header globally (Like Postman)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your JWT token}' below. Example: Bearer eyJhbGciOiJIUz..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Reads the connection string dynamically from environment variables
// Render provides the database connection string as an environment variable.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? throw new InvalidOperationException("Database Connection Is Missing.");


builder.Services.AddDbContext<FarmDbContext>(options => 
    options.UseNpgsql(connectionString));


// Register repositories
builder.Services.AddScoped<IFarmRepository, FarmRepository>();  // Repository for DB access
builder.Services.AddScoped<IFarmerRepository, FarmerRepository>();  // Repository for DB access
builder.Services.AddScoped<IFieldRepository, FieldRepository>();  // Repository for DB access


// Register services
builder.Services.AddScoped<JwtService>();// Register JwtService
builder.Services.AddScoped<IFarmService, FarmService>(); // Service for business logic`
builder.Services.AddScoped<IFarmerService, FarmerService>(); // Service for business logic
builder.Services.AddScoped<IFieldService, FieldService>(); // Service for business logic

//For Controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigration(); //Automatically Apply migrations
}

if (app.Environment.IsProduction())
{
    app.ApplyMigration(); //Automatically Apply migrations
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
//app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Welcome}/{action=Welcome}/{id?}"); // Default route


app.Run();
