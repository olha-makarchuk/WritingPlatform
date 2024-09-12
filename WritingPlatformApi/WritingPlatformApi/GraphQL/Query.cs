using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WritingPlatformApi.GraphQL;

namespace Application.PlatformFeatures.GraphQL
{
    public class Query: BaseGraphQL
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
         
        public Query(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IEnumerable<Genre>> GetGenresAsync()
        {
            return await _context.Genre.ToListAsync();
        }

        public async Task<IEnumerable<Publication>> GetPublicationsAsync()
        {
            return await _context.Publication.Include(u=>u.Genre).ToListAsync();
        }
    }
}
