using Application.Interfaces;
using Application.Services;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteAccountCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId)
                 ?? throw new NotFoundException("User not found");

            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            return user;
        }
    }
}
