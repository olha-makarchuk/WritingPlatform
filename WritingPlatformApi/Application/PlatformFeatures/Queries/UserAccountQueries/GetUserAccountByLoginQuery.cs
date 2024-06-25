using Application.Interfaces;
using Application.Services;
using Contracts.Responses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.PlatformFeatures.Queries.UserAccountQueries
{
    public class GetUserAccountByLoginQuery : IRequest<PersonalInformationResponse>
    {
        public string UserId { get; set; }
    }

    public class GetUserAccountByLoginQueryHandler : IRequestHandler<GetUserAccountByLoginQuery, PersonalInformationResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GetUserAccountByLoginQueryHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<PersonalInformationResponse> Handle(GetUserAccountByLoginQuery query, CancellationToken cancellationToken)
        {
            var user = _userManager.FindByIdAsync(query.UserId).Result
                ?? throw new NotFoundException("User not found");

            PersonalInformationResponse response = new();
            response.UserName = user.UserName;
            response.Email = user.Email;
            response.PersonalInformation = user.PersonalInformation;
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            return response;
        }
    }
}
