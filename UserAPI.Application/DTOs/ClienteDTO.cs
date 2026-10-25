using System.ComponentModel.DataAnnotations;

namespace UserAPI.Application.DTOs
{
    public class ClienteDTO
    {
        [Required(ErrorMessage = "La clave es requerida")]
        [MaxLength(8, ErrorMessage = "La clave no puede superar los 8 dígitos")]
        public string? Clave { get; set; }
        public string? Nombre { get; set; }
        public int Edad { get; set; }
        public DateOnly Fecha_Nacimiento { get; set; }
    }
}
