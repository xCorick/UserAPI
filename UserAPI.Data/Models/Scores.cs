using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Core.Models
{
    public class Scores : BaseEntity
    {
        public decimal Score { get; set; }
        public bool IsMixed { get; set; }
        public int Points { get; set; }
        public Guid? IdSubject { get; set; }
        public Guid IdUser { get; set; }
        public Subjects? Subject {  get; set; }
    }
}
