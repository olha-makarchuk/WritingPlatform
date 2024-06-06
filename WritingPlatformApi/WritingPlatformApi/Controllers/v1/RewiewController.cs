using Application.PlatformFeatures.Commands.RewiewCommand;
using Application.PlatformFeatures.Queries.Rewiew;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class RewiewController : BaseApiController
    {
        public RewiewController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        [Route("rewiew")]
        public async Task<IActionResult> CreateOwnRewiew(CreateRewiewCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost]
        [Route("own-rewiew")]
        public async Task<IActionResult> GetOwnRewiew(GetOwnRewiewQuery query)
        {
            var a = await Mediator.Send(query);
            return Ok(a);
        }

        [HttpDelete("{rewiewId}")]
        public async Task<IActionResult> DeleteRewiew(int rewiewId)
        {
            return Ok(await Mediator.Send(new DeleteRewiewCommand { RewiewId = rewiewId }));
        }
    }
}
