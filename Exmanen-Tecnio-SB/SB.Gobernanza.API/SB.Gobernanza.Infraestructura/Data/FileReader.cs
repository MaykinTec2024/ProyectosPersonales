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
    public class FileReader : IFileReader
    {
        public IEnumerable<Entidad> ReadEntitiesFromFile(string filePath)
        {
            var entities = new List<Entidad>();

            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(','); 
                if (parts.Length == 7)
                {
                    entities.Add(new Entidad
                    {
                        Id = int.Parse(parts[0].Trim()),
                        Nombre = parts[1].Trim(),
                        Tipo = parts[2].Trim(),
                        Direccion = parts[3].Trim(),
                        Ciudad = parts[4].Trim(),
                        Telefono = parts[5].Trim(),
                        CorreoElectronico = parts[6].Trim()
                    });
                }
            }

            return entities;
        }
    }
}
