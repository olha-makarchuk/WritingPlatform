using Application.PlatformFeatures.Commands.GenreCommands;
using Application.PlatformFeatures.Queries.GenreQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class GenreController: BaseApiController
    {
        public GenreController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var a = await Mediator.Send(new GetAllGenreQuery());
            return Ok(a);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGenreCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
