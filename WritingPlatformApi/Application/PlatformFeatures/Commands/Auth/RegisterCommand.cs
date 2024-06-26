using Contracts.Response.Auth;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Application.PlatformFeatures.Commands.Auth
{
    public class RegistrationCommand : IRequest<RegistrationResponse>
    {
        [Required(ErrorMessage = "UserName is required.")]
        [MaxLength(256, ErrorMessage = "UserName cannot be longer than 256 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "FirstName is required.")]
        [MaxLength(256, ErrorMessage = "FirstName cannot be longer than 256 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required.")]
        [MaxLength(256, ErrorMessage = "LastName cannot be longer than 256 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(256, ErrorMessage = "Email cannot be longer than 256 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [MaxLength(256, ErrorMessage = "Role cannot be longer than 256 characters.")]
        public string Role { get; set; }
    }

    public class RegistrationCommandHandler : IRequestHandler<RegistrationCommand, RegistrationResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegistrationCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<RegistrationResponse> Handle(RegistrationCommand request, CancellationToken cancellationToken = default)
        {
            var userExists = await _userManager.FindByNameAsync(request.UserName);

            if (userExists != null) throw new Exception("User already exists");

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsAuthor = false,
                IsActive = true,
                Email = request.Email,
                RefreshToken = GenerateRefreshToken(),
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.UserName,
            };

            var createdUserResult = await _userManager.CreateAsync(user, request.Password);

            if (!createdUserResult.Succeeded) throw new Exception("User creation failed! Please check user details and try again.");

            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(request.Role));
            }

            if (await _roleManager.RoleExistsAsync(request.Role))
            {
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            var registeredUser = await _userManager.FindByNameAsync(request.UserName);

            return new RegistrationResponse
            {
                UserId = registeredUser.Id,
                UserName = registeredUser.UserName,
                Email = registeredUser.Email
            };
        }
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
