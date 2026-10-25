using System.ComponentModel.DataAnnotations;

namespace UserAPI.Application.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string? Password { get; set; }
    }
}
