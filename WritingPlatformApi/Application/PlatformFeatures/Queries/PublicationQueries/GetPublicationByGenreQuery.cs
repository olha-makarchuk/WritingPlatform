using Application.Interfaces;
using Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationByGenreQuery : IRequest<List<PublicationResponse>>
    {
        public int IdGenre { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public class GetMovieByIdQueryHandler : IRequestHandler<GetPublicationByGenreQuery, List<PublicationResponse>>
        {
            private readonly IApplicationDbContext _context;

            public GetMovieByIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<PublicationResponse>> Handle(GetPublicationByGenreQuery query, CancellationToken cancellationToken)
            {
                var genreExist = await _context.Genre.Where(a => a.Id == query.IdGenre)
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new Exception("Genre not found");

                var publicationList = await _context.Publication
               .Where(a => a.GenreId == query.IdGenre)
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
                   bookDescription = p.bookDescription
               })
               .ToListAsync(cancellationToken)
                ?? throw new Exception("Publication not found");

                return publicationList;
            }
        }
    }
}
