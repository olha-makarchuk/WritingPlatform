using Application.Interfaces;
using Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetAllPublicationQuery : IRequest<IEnumerable<PublicationResponse>>
    {
        [Required(ErrorMessage = "PageNumber is required.")]
        public int PageNumber { get; set; }

        [Required(ErrorMessage = "PageSize is required.")]
        public int PageSize { get; set; }
    }

    public class GetAllPublicationQueryHandler : IRequestHandler<GetAllPublicationQuery, IEnumerable<PublicationResponse>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllPublicationQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PublicationResponse>> Handle(GetAllPublicationQuery query, CancellationToken cancellationToken)
        {
            var totalPublications = await _context.Publication.CountAsync();

            var publicationList = await _context.Publication
                .Include(p => p.Genre)
                .Include(p => p.ApplicationUser)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .OrderBy(g => g.PublicationName)
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
                    bookDescription = p.bookDescription,
                    PaginatorCount = (int)Math.Ceiling((double)totalPublications / query.PageSize) 
                })
                .ToListAsync(cancellationToken);

            return publicationList;
        }
    }
}
