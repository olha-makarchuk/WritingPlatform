using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Queries.UserAccountQueries
{
    public class GetUserAccountByEmailQuery : IRequest<ApplicationUser>
    {
        public string Email { get; set; }
    }
    /*
    public class GetUserAccountByEmailQueryHandler :  IRequestHandler<GetUserAccountByEmailQuery, ApplicationUser>
    {
        private readonly IApplicationDbContext _context;

        public GetUserAccountByEmailQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> Handle(GetUserAccountByEmailQuery query, CancellationToken cancellationToken)
        {
            return await _context.ApplicationUser.Where(a => a.Email == query.Email)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("User not found");
        }
    }*/
}
