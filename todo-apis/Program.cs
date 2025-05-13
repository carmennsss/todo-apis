using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using todo_apis.Context;
using todo_apis.Services;
using todo_apis.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var origenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(',');


builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(politica =>
    {
        politica.WithOrigins(origenesPermitidos)
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
