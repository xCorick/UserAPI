namespace UserAPI.Domain.Entities
{
    public class BaseEntity
    {
        public Guid? Id { get; set; }
        public DateTime Created_At { get; set; }
        public bool IsActive { get; set; }
    }
}
