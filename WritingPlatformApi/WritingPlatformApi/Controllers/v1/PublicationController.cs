using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.PlatformFeatures.Queries.PublicationQueries;
using Application.PlatformFeatures.Commands.PublicationCommands;
using Application.PlatformFeatures.Commands.RewiewCommand;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PublicationController : BaseApiController
    {
        private readonly ILogger<PublicationController> _logger;

        public PublicationController(ILogger<PublicationController> logger, IMediator mediator) : base(mediator)
        {
            _logger = logger;
        }        [HttpPost]
        public async Task<IActionResult> Create(CreatePublicationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPost]
        [Route("by-genre")]
        public async Task<IActionResult> GetPubicationsByGenre(GetPublicationByGenreQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpPost]
        [Route("by-author")]
        public async Task<IActionResult> GetPubicationsByAuthor(GetPublicationByAuthorQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpPost]
        [Route("all-publication")]//////////////////////
        public async Task<IActionResult> GetAllPubications(GetAllPublicationQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpPost]
        [Route("by-id")]
        public async Task<IActionResult> GetPubicationsById(GetPublicationByIdQuery query)
        {
            var a = await Mediator.Send(query);
            return Ok(a);
        }

        [HttpDelete("{publicationId}")]
        public async Task<IActionResult> DeletePubicationById(int publicationId)
        {
            return Ok(await Mediator.Send(new DeletePublicationByIdCommand() { Id= publicationId }));
        }

        [HttpPost]
        [Route("rewiew")]
        public async Task<IActionResult> AddRewiew(CreateRewiewCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        [HttpPost]
        [Route("by-name")]
        public async Task<IActionResult> GetPubicationsByName(GetPublicationByNameQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
