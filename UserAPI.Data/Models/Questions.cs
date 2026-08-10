using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Core.Models
{
    public class Questions : BaseEntity
    {
        public string? Question {  get; set; }
        public Guid? IdSubject { get; set; }
        public List<Options>? Options { get; set; } = [];
        [NotMapped]
        public string? SubjectName { get; set; }
    }
}
