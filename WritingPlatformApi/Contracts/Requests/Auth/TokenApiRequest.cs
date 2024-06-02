using System.ComponentModel.DataAnnotations;

namespace Contracts.Requests.Auth
{
    public class TokenApiRequest
    {
        [Required(ErrorMessage = "Access Token is required")]
        public string AccessToken { get; set; }

        [Required(ErrorMessage = "Refresh Token is required")]
        public string RefreshToken { get; set; }
    }
}
