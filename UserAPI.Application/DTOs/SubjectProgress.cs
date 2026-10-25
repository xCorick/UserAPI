namespace UserAPI.Application.DTOs
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
