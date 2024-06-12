﻿using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Contracts.Responses;

namespace Application.PlatformFeatures.Queries.PublicationQueries
{
    public class GetPublicationByIdQuery : IRequest<PublicationByIdResponse>
    {
        public int IdPublication { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; } 

        public class GetPublicationByIdQueryHandler : IRequestHandler<GetPublicationByIdQuery, PublicationByIdResponse>
        {
            private readonly IApplicationDbContext _context;

            public GetPublicationByIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<PublicationByIdResponse> Handle(GetPublicationByIdQuery query, CancellationToken cancellationToken)
            {
                var publication = await _context.Publication
               .Where(a => a.Id == query.IdPublication)
               .Include(p => p.Genre)
               .Include(p => p.ApplicationUser)
               .Skip((query.PageNumber - 1) * query.PageSize)
               .Take(query.PageSize)
               .Select(p => new PublicationByIdResponse
               {
                   PublicationId = p.Id,
                   PublicationName = p.PublicationName,
                   GenreName = p.Genre.Name,
                   Rating = p.Rating,
                   FileKey = p.FileKey,
                   TitleKey = p.TitleKey,
                   CountOfPages = p.CountOfPages,
                   CountOfRewiews = p.CountOfRewiews,
                   Author = new AuthorResponse
                   {
                       UserName = p.ApplicationUser.UserName,
                       FirstName = p.ApplicationUser.FirstName,
                       LastName = p.ApplicationUser.LastName,
                       PersonalInformation = p.ApplicationUser.PersonalInformation
                   },
                   bookDescription = p.bookDescription,
                   DatePublication = p.DatePublication
               })
               .FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("Publication not found");

                return publication;
            }
        }
    }
}
