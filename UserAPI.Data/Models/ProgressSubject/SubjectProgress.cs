using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAPI.Core.Models.SubjectProgress
{
    public class SubjectProgress
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal AverageScore { get; set; }

        public int TotalPoints { get; set; }

        public int GamesPlayed { get; set; }
    }
}
