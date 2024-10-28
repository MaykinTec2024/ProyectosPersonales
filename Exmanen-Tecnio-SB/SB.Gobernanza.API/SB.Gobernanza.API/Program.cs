using Aplicacion.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SB.Gobernanza.Dominio.Interfaces;
using SB.Gobernanza.Infraestructura.Data;
using Serilog; // Asegúrate de incluir esto
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console() // Registra en la consola
    .WriteTo.File("logs\\app.log", rollingInterval: RollingInterval.Day) // Registra en un archivo
    .CreateLogger();

builder.Host.UseSerilog(); // Usa Serilog como el logger

// Add services to the container.
builder.Services.AddControllers();

// Agregar configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5173") // Cambia esto a la URL de tu aplicación React
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Agregar servicios antes de construir la aplicación
builder.Services.AddScoped<IFileReader, FileReader>();
builder.Services.AddScoped<IFileReaderUsuario, FileReaderUsuario>(); // Registra la implementación concreta
builder.Services.AddScoped<EntityService>();
builder.Services.AddScoped<UsuarioService>();

// Configuración de autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero // Evita el desfase de tiempo en la validación del token
    };
});

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

// Habilita la autenticación y autorización
app.UseAuthentication(); // Debe ir antes de UseAuthorization
app.UseAuthorization();

// Habilitar CORS
app.UseCors("AllowSpecificOrigin"); // Asegúrate de que esto esté antes de MapControllers

app.MapControllers();

app.Run();
