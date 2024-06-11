using Application.PlatformFeatures.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AuthenticationController : BaseApiController
    {
        private readonly ILogger<PublicationController> _logger;

        public AuthenticationController(ILogger<PublicationController> logger, IMediator mediator) : base(mediator)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginCommand command)
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

        [HttpPost]
        [Route("registration")]
        public async Task<IActionResult> Register(RegistrationCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");

                var user = await Mediator.Send(command);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
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

        [HttpGet]
        public async Task<IActionResult> GetHello()
        {
            return Ok("Hell");
        }
    }
}
