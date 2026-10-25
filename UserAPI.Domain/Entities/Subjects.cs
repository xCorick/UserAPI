using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAPI.Domain.Entities
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
