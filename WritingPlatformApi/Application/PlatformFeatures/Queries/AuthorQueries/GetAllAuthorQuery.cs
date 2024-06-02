using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.AuthorQueries
{
    public class GetAllAuthorQuery : IRequest<IEnumerable<Author>>
    {
        public class GetAllMoviesQueryHandler : IRequestHandler<GetAllAuthorQuery, IEnumerable<Author>>
        {
            private readonly IApplicationDbContext _context;
            private IApiClientGoogleDrive _client;
            private readonly UserManager<ApplicationUser> _userManager;

            public GetAllMoviesQueryHandler(IApplicationDbContext context, IApiClientGoogleDrive client, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _client = client;
                _userManager = userManager;
            }

            public async Task<IEnumerable<Author>> Handle(GetAllAuthorQuery request, CancellationToken cancellationToken)
            {
                var authors = await _context.Publication
                        .Include(p => p.ApplicationUser)
                        .GroupBy(p => new { p.ApplicationUserId, p.ApplicationUser.FirstName, p.ApplicationUser.LastName, p.ApplicationUser.UserName})
                        .Select(g => new Author
                        {
                            IdAuthor = g.Key.ApplicationUserId,
                            UserName = g.Key.UserName,
                            FirstName = g.Key.FirstName,
                            LastName = g.Key.LastName,
                            CountPublication = g.Count()
                        })
                        .ToListAsync(cancellationToken);


                return authors.AsReadOnly();
            }
        }
    }

    public class Author
    {
        public string IdAuthor {  get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName {  get; set; } = string.Empty;
        public int CountPublication {  get; set; }
    }
}
