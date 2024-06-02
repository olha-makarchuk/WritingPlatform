using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationTextQuery : IRequest<PublicationText>
    {
        public int IdPublication { get; set; }
        public int CurrentPage { get; set; }

        public class GetPublicationTextQueryHandler :  IRequestHandler<GetPublicationTextQuery, PublicationText>
        {
            private readonly IApplicationDbContext _context;
            private IApiClientGoogleDrive _client;

            public GetPublicationTextQueryHandler(IApplicationDbContext context, IApiClientGoogleDrive client)
            {
                _context = context;
                _client = client;
            }

            public async Task<PublicationText> Handle(GetPublicationTextQuery query, CancellationToken cancellationToken)
            {
                var publication = await _context.Publication
                    .Where(p => p.Id == query.IdPublication)
                    .FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("Publication not found");

                string text = _client.GetTextFromDocxFile(publication.FileKey, query.CurrentPage);

                var publicationText = new PublicationText()
                {
                    PublicationId = publication.Id,
                    PublicationName = publication.PublicationName,
                    Text = text,
                    CountOfPages = publication.CountPages,
                    CurrentPage = query.CurrentPage
                };

                return publicationText;
            }
        }
    }

    public class PublicationText
    {
        public int PublicationId { get; set; }
        public string PublicationName { get; set; }
        public string Text { get; set; }
        public int CountOfPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
