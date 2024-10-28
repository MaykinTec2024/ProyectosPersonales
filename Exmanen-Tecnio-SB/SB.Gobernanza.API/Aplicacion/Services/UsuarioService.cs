using SB.Gobernanza.Dominio.Interfaces;
using SB.Gobernanza.Dominio.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Services
{
    public class UsuarioService
    {
        private readonly IFileReaderUsuario _fileReader;

        public UsuarioService(IFileReaderUsuario fileReader)
        {
            _fileReader = fileReader;
        }
        public IEnumerable<Usuario> GetAllUsuarios(string filePath)
        {
            return _fileReader.ReadUsuariosFromFile(filePath);
        }

    }
}
