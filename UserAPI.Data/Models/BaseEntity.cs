using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Core.Models
{
    public class BaseEntity
    {
        public Guid? Id { get; set; }
        public DateTime Created_At { get; set; }
        public bool IsActive { get; set; }
    }
}
