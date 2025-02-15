﻿using Application.Interfaces;
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

            public GetAllGenressQueryHandler(IApplicationDbContext context )
            {
                _context = context;
            }
            
            public async Task<IEnumerable<Genre>> Handle(GetAllGenreQuery request, CancellationToken cancellationToken)
            {
                var genreList = await _context.Genre.ToListAsync(cancellationToken);

                return genreList.AsReadOnly();
            }
        }
    }
}
