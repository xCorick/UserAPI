using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Core.Models
{
    public class User
    {
        public Guid Id {  get; set; }
        [Required(ErrorMessage = "El nombre personal de usuario es requerido")]
        public string? Name { get; set; }
        [Required (ErrorMessage = "El nombre de usuario es requerido")]
        public string? UserName { get; set; }
        //[Required (ErrorMessage = "La contraseña es requerida")]
        public string? Password { get; set; }
        [Required (ErrorMessage = "El rol es requerido")]
        public RoleEnum Rol { get; set; }
        public Scores? Score { get; set; }
    }
}
