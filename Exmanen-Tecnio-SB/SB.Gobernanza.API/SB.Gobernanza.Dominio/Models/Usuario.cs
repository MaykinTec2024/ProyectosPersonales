using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Gobernanza.Dominio.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string UsuarioNombre { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }

        public static List<Usuario> CargarUsuariosDesdeArchivo(string rutaArchivo)
        {
            var usuarios = new List<Usuario>();

            foreach (var linea in File.ReadAllLines(rutaArchivo))
            {
                var datos = linea.Split(',');

                if (datos.Length == 4)
                {
                    var usuario = new Usuario
                    {
                        IdUsuario = int.Parse(datos[0].Trim()),
                        UsuarioNombre = datos[1].Trim(),
                        Password = datos[2].Trim(), 
                        Rol = datos[3].Trim()
                    };

                    usuarios.Add(usuario);
                }
            }

            return usuarios;
        }

    }
}
