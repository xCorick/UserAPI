namespace UserAPI.Application.DTOs
{
    public class QuestionPaginationFilters : PaginationFilter
    {
        public Guid? IdSubject { get; set; }
    }
}
