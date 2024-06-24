﻿using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.Rewiew
{
    public class GetOwnRewiewQuery : IRequest<UserRewiew>
    {
        public string UserId { get; set; }
        public int PublicationId { get; set; }

        public class GetOwnRewiewQueryHandler : IRequestHandler<GetOwnRewiewQuery, UserRewiew>
        {
            private readonly IApplicationDbContext _context;

            public GetOwnRewiewQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<UserRewiew> Handle(GetOwnRewiewQuery query, CancellationToken cancellationToken)
            {
                var publication = await _context.Publication.FirstOrDefaultAsync(u => u.Id == query.PublicationId)
                    ?? throw new Exception("Publication not found");

                var rewiew = await _context.UserRewiew
                    .Where(u => u.ApplicationUserId == query.UserId)
                    .Where(p => p.PublicationId == query.PublicationId)
                    .FirstOrDefaultAsync();

                return rewiew;
            }
        }
    }
}
