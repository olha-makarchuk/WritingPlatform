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
        
        [HttpPost]
        public async Task<IActionResult> GetAllAuthors(GetAllAuthorQuery query)
        {
            var authors = await Mediator.Send(query);

            return Ok(authors);
        }
    }
}
