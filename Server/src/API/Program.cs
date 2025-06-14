using Company;
using Education;
using EmployeeModule;
using Infrastructure.DbContext;
using Shared.Helpers;
using AuthModule;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;

var builder = WebApplication.CreateBuilder(args);

// Initialize Logger
LoggerHelper.Initialize(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

// Configure Database Connections
var tanTamConnection = builder.Configuration.GetConnectionString("TanTamConnection") 
    ?? throw new InvalidOperationException("TanTamConnection string is missing in configuration.");
var connectionStrings = new Dictionary<string, string>
{
    { "TanTam", tanTamConnection },
};
builder.Services.AddSingleton(new DatabaseConnection(connectionStrings));

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
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Add authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run(); 