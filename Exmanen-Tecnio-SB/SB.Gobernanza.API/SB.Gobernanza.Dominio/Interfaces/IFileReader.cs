using SB.Gobernanza.Dominio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Gobernanza.Dominio.Interfaces
{
    public interface IFileReader
    {
        IEnumerable<Entidad> ReadEntitiesFromFile(string filePath);
    }
}
