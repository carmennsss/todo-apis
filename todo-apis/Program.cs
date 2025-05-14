using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using todo_apis.Context;
using todo_apis.Services;
using todo_apis.Services.Interfaces;
using NSwag;
using NSwag.Generation.Processors.Security;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// DB
var connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// Controllers
builder.Services.AddControllers();

// NSwag (Swagger)
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Todo API";
    config.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Bearer"
    });

    config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

// CORS
var allowedUrls = builder.Configuration.GetValue<string>("AllowedUrls")!.Split(',');
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedUrls)
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

// Servicios
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();       // Sirve swagger/v1/swagger.json
    app.UseSwaggerUi();    // UI en /swagger
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
