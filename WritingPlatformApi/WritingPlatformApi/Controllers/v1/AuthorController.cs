using Application.PlatformFeatures.Queries.AuthorQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AuthorController : BaseApiController
    {
        public AuthorController(IMediator mediator) : base(mediator)
        {
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await Mediator.Send(new GetAllAuthorQuery());

            return Ok(authors);
        }
    }
}
