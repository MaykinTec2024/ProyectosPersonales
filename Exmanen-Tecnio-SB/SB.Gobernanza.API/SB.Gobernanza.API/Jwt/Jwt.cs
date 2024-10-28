using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SB.Gobernanza.Dominio.Models
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }

        public static TokenValidationResult ValidarToken(ClaimsIdentity identity, string filePath)
        {
            try
            {
                int claimsCount = identity.Claims.Count();

                if (claimsCount == 0)
                {
                    return new TokenValidationResult
                    {
                        Success = false,
                        Message = "Verificar si estás enviando un token válido",
                        Result = null
                    };
                }

                var idClaim = identity.Claims.FirstOrDefault(x => x.Type == "idUsuario");
                if (idClaim == null)
                {
                    return new TokenValidationResult
                    {
                        Success = false,
                        Message = "No se encontró el claim 'idUsuario'",
                        Result = null
                    };
                }
                var usuarios = Usuario.CargarUsuariosDesdeArchivo(filePath);
                Usuario usuario = Usuario.CargarUsuariosDesdeArchivo(filePath).FirstOrDefault(x => x.IdUsuario.ToString() == idClaim.Value);

                return new TokenValidationResult
                {
                    Success = usuario != null,
                    Message = usuario != null ? "Éxito" : "Usuario no encontrado",
                    Result = usuario
                };
            }
            catch (Exception e)
            {
                return new TokenValidationResult
                {
                    Success = false,
                    Message = $"Error: {e.Message}",
                    Result = null
                };
            }
        }
    }
}
