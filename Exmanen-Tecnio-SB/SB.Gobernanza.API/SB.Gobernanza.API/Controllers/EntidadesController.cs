using Aplicacion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SB.Gobernanza.Dominio.Models;
using System.IO;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class EntitiesController : ControllerBase
{
    private readonly EntityService _entityService;
    private readonly string _filePath;
    private readonly string _filePathUsers;
    private readonly ILogger<EntitiesController> _logger;

    public EntitiesController(EntityService entityService, IConfiguration configuration, ILogger<EntitiesController> logger)
    {
        _entityService = entityService;
        _filePath = configuration["FileSettings:EntitiesFilePath"];
        _filePathUsers = configuration["FileSettings:UsuariosFilePath"];
        _logger = logger;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetEntities()
    {
        try
        {
            _logger.LogInformation("Solicitando la lista de entidades.");
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            var rToken = Jwt.ValidarToken(identity, _filePathUsers);

            if (!rToken.Success)
            {
                _logger.LogWarning("Token no válido: {Message}", rToken.Message);
                return Unauthorized(new { message = rToken.Message });
            }

            Usuario usuario = rToken.Result;

            var entities = await _entityService.GetAllEntitiesAsync(_filePath);
            _logger.LogInformation("Entidades recuperadas exitosamente: {Count}", entities.Count());

            return Ok(entities);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Error al acceder a la base de datos.");
            return StatusCode(500, new { message = "Error al acceder a la base de datos.", details = dbEx.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se produjo un error inesperado.");
            return StatusCode(500, new { message = "Se produjo un error inesperado.", details = ex.Message });
        }
    }


    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntityById(int id)
    {
        try
        {
            _logger.LogInformation("Solicitando la entidad con ID: {Id}", id);
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            var rToken = Jwt.ValidarToken(identity, _filePathUsers);

            if (!rToken.Success)
            {
                _logger.LogWarning("Token no válido: {Message}", rToken.Message);
                return Unauthorized(new { message = rToken.Message });
            }

            Usuario usuario = rToken.Result;

            var entity = await _entityService.GetEntityByIdAsync(id, _filePath);

            if (entity == null)
            {
                _logger.LogWarning("Entidad no encontrada con ID: {Id}", id);
                return NotFound(new { message = $"Entidad con ID {id} no encontrada." });
            }

            _logger.LogInformation("Entidad recuperada exitosamente con ID: {Id}", id);

            return Ok(entity);
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Error al acceder a la base de datos.");
            return StatusCode(500, new { message = "Error al acceder a la base de datos.", details = dbEx.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se produjo un error inesperado.");
            return StatusCode(500, new { message = "Se produjo un error inesperado.", details = ex.Message });
        }
    }


    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateEntity([FromBody] Entidad newEntity)
    {
        if (newEntity == null)
        {
            _logger.LogWarning("La entidad recibida es nula.");
            return BadRequest(new { message = "La entidad no puede ser nula." });
        }

        try
        {
            _logger.LogInformation("Intentando crear una nueva entidad: {Entity}", newEntity);
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.ValidarToken(identity, _filePathUsers);

            if (!rToken.Success)
            {
                _logger.LogWarning("Token no válido: {Message}", rToken.Message);
                return Unauthorized(new { message = rToken.Message });
            }

            Usuario usuario = rToken.Result;

            if (usuario.Rol != "admin")
            {
                _logger.LogWarning("Acceso denegado para el usuario: {UserId}", usuario.IdUsuario);
                return StatusCode(403, new { message = "Acceso denegado. Solo los administradores pueden agregar nuevas entidades." });
            }

            await _entityService.AddEntityAsync(newEntity, _filePath);
            _logger.LogInformation("Entidad creada exitosamente: {EntityId}", newEntity.Id);

            return CreatedAtAction(nameof(GetEntities), new { id = newEntity.Id }, newEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se produjo un error inesperado al crear la entidad.");
            return StatusCode(500, new { message = "Se produjo un error inesperado.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEntity(int id, [FromBody] Entidad updatedEntity)
    {
        Console.WriteLine($"ID de la entidad enviada: {updatedEntity?.Id}");
        Console.WriteLine($"ID esperado: {id}");

        if (updatedEntity == null || updatedEntity.Id != id)
        {
            _logger.LogWarning("Entidad no válida: {UpdatedEntityId}, Esperado: {ExpectedId}", updatedEntity?.Id, id);
            return BadRequest(new { message = "Entidad no válida." });
        }

        try
        {
            _logger.LogInformation("Intentando actualizar la entidad: {EntityId}", id);
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.ValidarToken(identity, _filePathUsers);

            if (!rToken.Success)
            {
                _logger.LogWarning("Token no válido: {Message}", rToken.Message);
                return Unauthorized(new { message = rToken.Message });
            }

            Usuario usuario = rToken.Result;

            if (usuario.Rol != "admin")
            {
                _logger.LogWarning("Acceso denegado para el usuario: {UserId}", usuario.IdUsuario);
                return StatusCode(403, new { message = "Acceso denegado. Solo los administradores pueden actualizar entidades." });
            }

            await _entityService.UpdateEntityAsync(updatedEntity, _filePath);
            _logger.LogInformation("Entidad actualizada exitosamente: {EntityId}", updatedEntity.Id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se produjo un error inesperado al actualizar la entidad.");
            return StatusCode(500, new { message = "Se produjo un error inesperado.", details = ex.Message });
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEntity(int id)
    {
        try
        {
            _logger.LogInformation("Intentando eliminar la entidad: {EntityId}", id);
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var rToken = Jwt.ValidarToken(identity, _filePathUsers);

            if (!rToken.Success)
            {
                _logger.LogWarning("Token no válido: {Message}", rToken.Message);
                return Unauthorized(new { message = rToken.Message });
            }

            Usuario usuario = rToken.Result;

            if (usuario.Rol != "admin")
            {
                _logger.LogWarning("Acceso denegado para el usuario: {UserId}", usuario.IdUsuario);
                return StatusCode(403, new { message = "Acceso denegado. Solo los administradores pueden eliminar entidades." });
            }

            await _entityService.DeleteEntityAsync(id, _filePath);
            _logger.LogInformation("Entidad eliminada exitosamente: {EntityId}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Se produjo un error inesperado al eliminar la entidad.");
            return StatusCode(500, new { message = "Se produjo un error inesperado.", details = ex.Message });
        }
    }
}
