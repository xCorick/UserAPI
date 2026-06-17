using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Data.DTOs
{
    public class ClienteDTO
    {
        [MaxLength(8, ErrorMessage = "La clave no puede superar los 8 dígitos")]
        public string? Clave { get; set; }
        public string? Nombre { get; set; }
        public int Edad { get; set; }
        public DateOnly Fecha_Nacimiento { get; set; }
    }
}
