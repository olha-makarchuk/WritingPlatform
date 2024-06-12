namespace Contracts.Responses
{
    public class PublicationByIdResponse
    {
        public int PublicationId { get; set; }
        public string PublicationName { get; set; }
        public string GenreName { get; set; }
        public int Rating { get; set; }
        public string TitleKey { get; set; }
        public string FileKey { get; set; }
        public DateTime DatePublication { get; set; }
        public AuthorResponse Author { get; set; }
        public string bookDescription { get; set; }
        public int CountOfPages { get; set; }
        public int CountOfRewiews { get; set; }
    }
}
