using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Contracts.Responses;
using Application.Services;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationByNameQuery : IRequest<List<PublicationResponse>>
    {
        public string PublicationName { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public class GetPublicationByNameQueryHandler : IRequestHandler<GetPublicationByNameQuery, List<PublicationResponse>>
        {
            private readonly IApplicationDbContext _context;
            public GetPublicationByNameQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<PublicationResponse>> Handle(GetPublicationByNameQuery query, CancellationToken cancellationToken)
            {
                var totalPublications = await _context.Publication.Where(u => u.PublicationName == query.PublicationName).CountAsync();

                var publicationList = await _context.Publication
                .Where(a => a.PublicationName == query.PublicationName)
                .Include(p => p.Genre)
                .Include(p => p.ApplicationUser)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(p => new PublicationResponse
                {
                    PublicationId = p.Id,
                    PublicationName = p.PublicationName,
                    GenreName = p.Genre.Name,
                    Author = new AuthorResponse
                    {
                        UserName = p.ApplicationUser.UserName,
                        FirstName = p.ApplicationUser.FirstName,
                        LastName = p.ApplicationUser.LastName,
                        PersonalInformation = p.ApplicationUser.PersonalInformation
                    },
                    Rating = p.Rating,
                    TitleKey = p.TitleKey,
                    FileKey = p.FileKey,
                    DatePublication = p.DatePublication,
                    bookDescription = p.bookDescription,
                    PaginatorCount = (int)Math.Ceiling((double)totalPublications / query.PageSize)
                })
                .ToListAsync(cancellationToken)
                 ?? throw new NotFoundException("Publication not found");

                return publicationList;
            }
        }
    }
}
