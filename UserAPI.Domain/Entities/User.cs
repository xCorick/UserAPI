using System.ComponentModel.DataAnnotations;
using UserAPI.Domain.Enums;

namespace UserAPI.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El nombre personal de usuario es requerido")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string? UserName { get; set; }
        public string? Password { get; set; }
        [Required(ErrorMessage = "El rol es requerido")]
        public RoleEnum Rol { get; set; }
        public Scores? Score { get; set; }
    }
}
