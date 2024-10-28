using SB.Gobernanza.Dominio.Interfaces;
using SB.Gobernanza.Dominio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Gobernanza.Infraestructura.Data
{
    public class FileReaderUsuario : IFileReaderUsuario
    {
        public IEnumerable<Usuario> ReadUsuariosFromFile(string filePath)
        {
            var usuario = new List<Usuario>();

            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(','); 
                if (parts.Length == 4)
                {
                    usuario.Add(new Usuario
                    {
                        IdUsuario = int.Parse(parts[0].Trim()),
                        UsuarioNombre = parts[1].Trim(),
                        Password = parts[2].Trim(),
                        Rol = parts[3].Trim()
                    });
                }
            }
            return usuario;
        }
    }
}
