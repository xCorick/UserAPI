using System.ComponentModel.DataAnnotations.Schema;

namespace UserAPI.Domain.Entities
{
    public class Questions : BaseEntity
    {
        public string? Question { get; set; }
        public Guid? IdSubject { get; set; }
        public List<Options>? Options { get; set; } = [];
        [NotMapped]
        public string? SubjectName { get; set; }
    }
}
