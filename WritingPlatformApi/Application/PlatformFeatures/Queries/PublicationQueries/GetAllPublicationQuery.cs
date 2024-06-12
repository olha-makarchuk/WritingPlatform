using Application.Interfaces;
using Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetAllPublicationQuery : IRequest<IEnumerable<PublicationResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public class GetAllMoviesQueryHandler : IRequestHandler<GetAllPublicationQuery, IEnumerable<PublicationResponse>>
        {
            private readonly IApplicationDbContext _context;

            public GetAllMoviesQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<PublicationResponse>> Handle(GetAllPublicationQuery request, CancellationToken cancellationToken)
            {
                var publicationList = await _context.Publication
                    .Include(p => p.Genre)
                    .Include(p => p.ApplicationUser)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(p => new PublicationResponse
                    {
                        PublicationId = p.Id,
                        PublicationName = p.PublicationName,
                        GenreName = p.Genre.Name,
                        Author = new AuthorResponse
                        {
                            FirstName = p.ApplicationUser.FirstName,
                            LastName = p.ApplicationUser.LastName,
                            UserName = p.ApplicationUser.UserName,
                            PersonalInformation = p.ApplicationUser.PersonalInformation
                        },
                        Rating = p.Rating,
                        TitleKey = p.TitleKey,
                        FileKey = p.FileKey,
                        DatePublication = p.DatePublication,
                        bookDescription = p.bookDescription
                    })
                    .ToListAsync(cancellationToken);

                return publicationList;
            }
        }
    }
}
