using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        protected BaseApiController(IMediator mediator)
        {
            _mediator = mediator;
        }
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
    }
}
