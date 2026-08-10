using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAPI.Data.Models;

namespace UserAPI.Core.Models.Pagination
{
    public class ScorePaginationFilters : PaginationFilter
    {
        public Guid IdUser {  get; set; }
    }
}
