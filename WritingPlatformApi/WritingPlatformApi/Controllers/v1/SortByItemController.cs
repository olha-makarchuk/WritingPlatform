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
            return Ok(await Mediator.Send(new GetAllSortByItemQuery()));
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
            return Ok(await Mediator.Send(query));
        }
    }
}
