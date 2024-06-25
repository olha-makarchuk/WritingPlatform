using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.PlatformFeatures.Queries.PublicationQueries;
using Application.PlatformFeatures.Commands.PublicationCommands;
using Microsoft.AspNetCore.Authorization;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class PublicationController : BaseApiController
    {
        private readonly ILogger<PublicationController> _logger;

        public PublicationController(ILogger<PublicationController> logger, IMediator mediator) : base(mediator)
        {
            _logger = logger;
        }

        [HttpPost]
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
        [Route("all-publication")]
        public async Task<IActionResult> GetAllP5ubications(GetAllPublicationQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpPost]
        [Route("by-id")]
        public async Task<IActionResult> GetPubicationsById(GetPublicationByIdQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpDelete("{publicationId}"), Authorize]
        public async Task<IActionResult> DeletePubicationById(int publicationId)
        {
            var result = await Mediator.Send(new DeletePublicationByIdCommand() { Id = publicationId });
            return Ok(result);
        }

        [HttpPost]
        [Route("by-name")]
        public async Task<IActionResult> GetPubicationsByName(GetPublicationByNameQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
