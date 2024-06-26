using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Commands.UserAccountCommands
{
    public class ChangePersonalInformationCommand: IRequest<ApplicationUser>
    {
        [Required(ErrorMessage = "PersonalInformation is required.")]
        public string PersonalInformation { get; set; } = string.Empty;

        [Required(ErrorMessage = "FirstName is required.")]
        [MaxLength(256, ErrorMessage = "FirstName cannot be longer than 256 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastName is required.")]
        [MaxLength(256, ErrorMessage = "LastName cannot be longer than 256 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserName is required.")]
        [MaxLength(256, ErrorMessage = "UserName cannot be longer than 256 characters.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserId is required.")]
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
                 ?? throw new NotFoundException("User not found");

            var userNameExists = await _userManager.FindByNameAsync(command.UserName);

            if(userNameExists != null && user.UserName != userNameExists.UserName)
            {
                throw new ArgumentException("User with the same username alredy exists");
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
