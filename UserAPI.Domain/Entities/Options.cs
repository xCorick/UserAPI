namespace UserAPI.Domain.Entities
{
    public class Options : BaseEntity
    {
        public string? Option { get; set; }
        public bool? IsAnswer { get; set; }
        public Guid? IdQuestion { get; set; }
    }
}
