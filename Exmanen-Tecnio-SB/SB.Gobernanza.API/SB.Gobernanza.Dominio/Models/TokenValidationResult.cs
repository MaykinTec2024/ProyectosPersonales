using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Gobernanza.Dominio.Models
{
    public class TokenValidationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Usuario Result { get; set; }
    }
}
