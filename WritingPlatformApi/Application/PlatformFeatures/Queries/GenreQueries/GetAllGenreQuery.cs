using Application.Interfaces;
using MediatR;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.GenreQueries
{
    public class GetAllGenreQuery : IRequest<IEnumerable<Genre>>
    {
        public class GetAllGenressQueryHandler : IRequestHandler<GetAllGenreQuery, IEnumerable<Genre>>
        {
            private readonly IApplicationDbContext _context;
            private IApiClientGoogleDrive _client;

            public GetAllGenressQueryHandler(IApplicationDbContext context, IApiClientGoogleDrive client)
            {
                _context = context;
                _client = client;
            }
            
            public async Task<IEnumerable<Genre>> Handle(GetAllGenreQuery request, CancellationToken cancellationToken)
            {
                var genreList = await _context.Genre.ToListAsync(cancellationToken);

                return genreList.AsReadOnly();
            }
        }
    }
}
