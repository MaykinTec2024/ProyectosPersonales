using Aplicacion.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SB.Gobernanza.Dominio.Interfaces;
using SB.Gobernanza.Infraestructura.Data;
using Serilog; // Aseg�rate de incluir esto
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console() // Registra en la consola
    .WriteTo.File("logs\\app.log", rollingInterval: RollingInterval.Day) // Registra en un archivo
    .CreateLogger();

builder.Host.UseSerilog(); // Usa Serilog como el logger

// Add services to the container.
builder.Services.AddControllers();

// Agregar configuraci�n de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:5173") // Cambia esto a la URL de tu aplicaci�n React
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Agregar servicios antes de construir la aplicaci�n
builder.Services.AddScoped<IFileReader, FileReader>();
builder.Services.AddScoped<IFileReaderUsuario, FileReaderUsuario>(); // Registra la implementaci�n concreta
builder.Services.AddScoped<EntityService>();
builder.Services.AddScoped<UsuarioService>();

// Configuraci�n de autenticaci�n JWT
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
        ClockSkew = TimeSpan.Zero // Evita el desfase de tiempo en la validaci�n del token
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

// Habilita la autenticaci�n y autorizaci�n
app.UseAuthentication(); // Debe ir antes de UseAuthorization
app.UseAuthorization();

// Habilitar CORS
app.UseCors("AllowSpecificOrigin"); // Aseg�rate de que esto est� antes de MapControllers

app.MapControllers();

app.Run();
