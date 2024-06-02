using Application.PlatformFeatures.Queries.Catalog;
using Application.PlatformFeatures.Queries.GenreQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CatalogController : BaseApiController
    {
        public CatalogController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost]
        public async Task<IActionResult> Sort(SortPublicationQuery query)
        {
            var a = await Mediator.Send(query);
            return Ok(a);
        }
    }
}
