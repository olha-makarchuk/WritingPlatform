using Application.PlatformFeatures.Commands.RewiewCommand;
using Application.PlatformFeatures.Queries.Review;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class RewiewController : BaseApiController
    {
        public RewiewController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost, Authorize]
        [Route("rewiew")]
        public async Task<IActionResult> CreateOwnRewiew(CreateRewiewCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost, Authorize]
        [Route("own-rewiew")]
        public async Task<IActionResult> GetOwnRewiew(GetOwnReviewQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpDelete("{rewiewId}"), Authorize]
        public async Task<IActionResult> DeleteRewiew(int rewiewId)
        {
            var result = await Mediator.Send(new DeleteRewiewCommand { RewiewId = rewiewId });
            return Ok(result);
        }
    }
}
