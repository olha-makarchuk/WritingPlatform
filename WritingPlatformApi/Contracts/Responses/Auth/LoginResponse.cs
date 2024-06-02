namespace Contracts.Response.Auth
{
    public class LoginResponse
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public TokenApiResponse TokenApiResponse {  get; set; }
    }
}
