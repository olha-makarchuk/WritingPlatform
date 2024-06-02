using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationByGenreQuery : IRequest<List<PublicationDto>>
    {
        public int IdGenre { get; set; }

        public class GetMovieByIdQueryHandler : IRequestHandler<GetPublicationByGenreQuery, List<PublicationDto>>
        {
            private readonly IApplicationDbContext _context;

            public GetMovieByIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<PublicationDto>> Handle(GetPublicationByGenreQuery query, CancellationToken cancellationToken)
            {
                var genreExist = await _context.Genre.Where(a => a.Id == query.IdGenre)
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new Exception("Genre not found");

                var publicationList = await _context.Publication
               .Where(a => a.GenreId == query.IdGenre)
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
