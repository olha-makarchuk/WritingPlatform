using Application.PlatformFeatures.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    public class TokenController:BaseApiController
    {
        private readonly ILogger<PublicationController> _logger;

        public TokenController(ILogger<PublicationController> logger, IMediator mediator) : base(mediator)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");

                var user = await Mediator.Send(command);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
