using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Core.Models.QuestionsDTOs
{
    public class SetQuestions
    {
        public Guid? IdSubject { get; set; }
        public int TotalQuestions { get; set; }
    }
}
