using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string PersonalInformation { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsAuthor { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhotoPath {  get; set; } = string.Empty;
    }
}
