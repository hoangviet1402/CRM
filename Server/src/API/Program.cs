using Company;
using Education;
using EmployeeModule;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using AuthModule;

var builder = WebApplication.CreateBuilder(args);

// Initialize Logger
LoggerHelper.Initialize(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add modules
builder.Services.AddEmployeeModule();
builder.Services.AddEducationModule();
builder.Services.AddCompanyModule();
builder.Services.AddAuthModule(builder.Configuration);

// Add Swagger
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

// Add authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run(); 