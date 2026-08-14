using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Core.Models.SubjectProgress
{
    public class UserGlobalProgress
    {
        public decimal GeneralScore { get; set; }

        public int SubjectsPlayed { get; set; }

        public int TotalPreguntas { get; set; }

        public int Streak { get; set; }

        public List<SubjectProgress> Subjects { get; set; } = new();
    }
}
