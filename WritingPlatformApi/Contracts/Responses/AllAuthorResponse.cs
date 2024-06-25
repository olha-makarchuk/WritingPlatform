namespace Contracts.Responses
{
    public class AllAuthorResponse
    {
        public string IdAuthor { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int CountPublication { get; set; }
        public int PaginatorCount { get; set; }
    }
}
