using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.PlatformFeatures.Commands.UserAccountCommands
{
    public class DeleteAccountCommand : IRequest<ApplicationUser>
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, ApplicationUser>
    {
        private readonly IApplicationDbContext _context;
        private IApiClientGoogleDrive _client;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteAccountCommandHandler(IApplicationDbContext context, IApiClientGoogleDrive client, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _client = client;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);

            user.IsActive = false;
            _userManager.UpdateAsync(user);

            return user;
        }
    }
}
