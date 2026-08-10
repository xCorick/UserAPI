using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Core.Models
{
    public class Options : BaseEntity
    {
        public string? Option { get; set; }
        public bool? IsAnswer { get; set; }
        public Guid? IdQuestion { get; set; }
    }
}
