using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Core.Models
{
    public class Subjects : BaseEntity
    {
        [Required(ErrorMessage = "El nombre de la materia es requerida")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "La descripción de la materia es requerida")]
        public string? Description { get; set; }
        [NotMapped]
        public int TotalQuestions { get; set; }
    }
}
