using Application.PlatformFeatures.Commands.UserAccountCommands;
using Application.PlatformFeatures.Queries.UserAccountQueries;
using Google.Apis.Drive.v3.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class UserAccountController : BaseApiController
    {
        public UserAccountController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        [Route("change")]
        public async Task<IActionResult> ChangePersonalInformation(ChangePersonalInformationCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [HttpPost]
        [Route("by-userId")]/////////////////////////
        public async Task<IActionResult> PersonalInformation(GetUserAccountByLoginQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteAccount(string accountId)
        {
            return Ok(await Mediator.Send(new DeleteAccountCommand() {UserId = accountId }));
        }
    }
}
