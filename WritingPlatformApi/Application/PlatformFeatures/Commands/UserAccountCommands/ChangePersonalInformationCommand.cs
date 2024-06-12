using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PlatformFeatures.Commands.UserAccountCommands
{
    public class ChangePersonalInformationCommand: IRequest<ApplicationUser>
    {
        public string PersonalInformation { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
    public class UpdateGenreCommandHandler : IRequestHandler<ChangePersonalInformationCommand, ApplicationUser>
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateGenreCommandHandler(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ApplicationUser> Handle(ChangePersonalInformationCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId)
                 ?? throw new Exception("User not found");

            var userNameExists = await _userManager.FindByNameAsync(command.UserName);

            if(userNameExists != null)
            {
                throw new Exception("User with the same username alredy exists");
            }

            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.PersonalInformation = command.PersonalInformation; 
            user.UserName = command.UserName;

            await _userManager.UpdateAsync(user);

            return user;
        }
    }
}
