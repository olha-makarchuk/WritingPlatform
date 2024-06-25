using Application.Interfaces;
using Contracts.Responses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.AuthorQueries
{
    public class GetAllAuthorQuery : IRequest<IEnumerable<AllAuthorResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public class GetAllAuthorQueryHandler : IRequestHandler<GetAllAuthorQuery, IEnumerable<AllAuthorResponse>>
        {
            private readonly IApplicationDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;

            public GetAllAuthorQueryHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            public async Task<IEnumerable<AllAuthorResponse>> Handle(GetAllAuthorQuery request, CancellationToken cancellationToken)
            {
                var totalAuthors = await _context.Publication
                    .Select(p => p.ApplicationUserId)
                    .Distinct()
                    .CountAsync(cancellationToken);

                var authors = await _context.Publication
                    .Include(p => p.ApplicationUser)
                    .GroupBy(p => new { p.ApplicationUserId, p.ApplicationUser.FirstName, p.ApplicationUser.LastName, p.ApplicationUser.UserName })
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(g => new AllAuthorResponse
                    {
                        IdAuthor = g.Key.ApplicationUserId,
                        UserName = g.Key.UserName,
                        FirstName = g.Key.FirstName,
                        LastName = g.Key.LastName,
                        CountPublication = g.Count(),
                        PaginatorCount = (int)Math.Ceiling((double)totalAuthors / request.PageSize)
                    })
                    .ToListAsync(cancellationToken);

                return authors;
            }
        }
    }
}
