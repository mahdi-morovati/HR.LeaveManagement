using HR.LeaveManagement.Api.Middleware;
using HR.LeaveManagement.Application;
using HR.LeaveManagement.Identity;
using HR.LeaveManagement.Infrastructure;
using HR.LeaveManagement.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in Development Mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();