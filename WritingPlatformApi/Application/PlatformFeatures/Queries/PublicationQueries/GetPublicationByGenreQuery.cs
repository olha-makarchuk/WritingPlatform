using Application.Interfaces;
using Application.Services;
using Contracts.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationByGenreQuery : IRequest<List<PublicationResponse>>
    {
        [Required(ErrorMessage = "IdGenre is required.")]
        public int IdGenre { get; set; }

        [Required(ErrorMessage = "PageNumber is required.")]
        public int PageNumber { get; set; }

        [Required(ErrorMessage = "PageSize is required.")]
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
                    ?? throw new NotFoundException("Genre not found");

                var totalPublications = await _context.Publication.Where(u => u.GenreId == query.IdGenre).CountAsync();

                var publicationList = await _context.Publication
               .Where(a => a.GenreId == query.IdGenre)
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
