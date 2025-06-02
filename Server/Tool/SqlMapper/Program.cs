using SqlMapper.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Cấu hình Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "SQL Mapper API", 
        Version = "v1",
        Description = "API để tự động tạo các lớp C# từ stored procedure"
    });
});

// Register our services
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IModelGeneratorService, ModelGeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SQL Mapper API v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
