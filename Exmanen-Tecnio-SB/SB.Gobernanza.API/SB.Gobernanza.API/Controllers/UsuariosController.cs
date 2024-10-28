using Aplicacion.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SB.Gobernanza.Dominio.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SB.Gobernanza.Servicios.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly string _filePath;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(UsuarioService usuarioService, IConfiguration configuration, ILogger<UsuariosController> logger)
        {
            _usuarioService = usuarioService;
            _configuration = configuration;
            _logger = logger;
            _filePath = configuration["FileSettings:UsuariosFilePath"];
        }

        [HttpPost]
        [Route("login")]
        public IActionResult IniciarSesion([FromBody] object optData)
        {
            var data = JsonConvert.DeserializeObject<dynamic>(optData.ToString());
            string user = data.usuario.ToString();
            string password = data.password.ToString();

            _logger.LogInformation($"Intento de inicio de sesión para el usuario: {user}");

            var usuarios = _usuarioService.GetAllUsuarios(_filePath);
            var usuario = usuarios.FirstOrDefault(x => x.UsuarioNombre == user && x.Password == password);

            if (usuario == null)
            {
                _logger.LogWarning($"Credenciales incorrectas para el usuario: {user}");
                return Unauthorized(new
                {
                    success = false,
                    message = "Credenciales Incorrectas",
                    token = ""
                });
            }

            var jwt = _configuration.GetSection("Jwt").Get<Jwt>();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("idUsuario", usuario.IdUsuario.ToString()),
                new Claim("usuarioNombre", usuario.UsuarioNombre),
                new Claim("rol", usuario.Rol)
            };

            _logger.LogInformation($"Inicio de sesión exitoso para el usuario: {usuario.UsuarioNombre}");

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
            var signingCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                jwt.Issuer,
                jwt.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signingCredentials
            );

            return Ok(new
            {
                success = true,
                message = "Éxito",
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [HttpGet]
        public IActionResult GetUsuarios()
        {
            _logger.LogInformation("Solicitud recibida para obtener todos los usuarios.");
            var usuarios = _usuarioService.GetAllUsuarios(_filePath);
            return Ok(usuarios);
        }
    }
}
