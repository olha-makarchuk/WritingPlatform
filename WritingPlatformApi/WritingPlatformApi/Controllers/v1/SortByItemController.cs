using Application.PlatformFeatures.Commands.SortByItemCommands;
using Application.PlatformFeatures.Queries.SortByItemQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    public class SortByItemController : BaseApiController
    {
        public SortByItemController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSortByItem()
        {
            var a = await Mediator.Send(new GetAllSortByItemQuery());
            return Ok(a);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSortByItem(CreateSortByItemCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        [HttpPost]
        [Route("sort")]
        public async Task<IActionResult> Sort(SortPublicationQuery query)
        {
            var a = await Mediator.Send(query);
            return Ok(a);
        }
    }
}
