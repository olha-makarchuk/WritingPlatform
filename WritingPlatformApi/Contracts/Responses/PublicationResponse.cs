namespace Contracts.Responses
{
    public class PublicationResponse
    {
        public int PublicationId { get; set; }
        public string PublicationName { get; set; }
        public string GenreName { get; set; }
        public AuthorResponse Author { get; set; }
        public int Rating { get; set; }
        public string FileKey { get; set; }
        public string TitleKey { get; set; }
        public DateTime DatePublication { get; set; }
        public string bookDescription { get; set; }
    }
}
