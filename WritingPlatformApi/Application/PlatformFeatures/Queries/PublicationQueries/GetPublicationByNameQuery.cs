using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationByNameQuery : IRequest<List<PublicationDto>>
    {
        public string PublicationName { get; set; } = string.Empty;

        public class GetPublicationByNameQueryHandler : IRequestHandler<GetPublicationByNameQuery, List<PublicationDto>>
        {
            private readonly IApplicationDbContext _context;
            public GetPublicationByNameQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<PublicationDto>> Handle(GetPublicationByNameQuery query, CancellationToken cancellationToken)
            {
                var publicationList = await _context.Publication
                .Where(a => a.PublicationName == query.PublicationName)
                .Include(p => p.Genre)
                .Include(p => p.ApplicationUser)
                .Select(p => new PublicationDto
                {
                    PublicationId = p.Id,
                    PublicationName = p.PublicationName,
                    GenreName = p.Genre.Name,
                    Author = new Author
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
                    bookDescription = p.bookDescription
                })
                .ToListAsync(cancellationToken)
                 ?? throw new Exception("Publication not found");

                return publicationList;
            }
        }
    }
}
