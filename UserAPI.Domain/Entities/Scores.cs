namespace UserAPI.Domain.Entities
{
    public class Scores : BaseEntity
    {
        public decimal Score { get; set; }
        public bool IsMixed { get; set; }
        public int Points { get; set; }
        public Guid? IdSubject { get; set; }
        public Guid IdUser { get; set; }
        public Subjects? Subject { get; set; }
    }
}
