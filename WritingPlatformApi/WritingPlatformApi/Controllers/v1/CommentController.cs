using Application.PlatformFeatures.Commands.CommentCommands;
using Application.PlatformFeatures.Queries.CommentQueries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CommentController : BaseApiController
    {
        public CommentController(IMediator mediator) : base(mediator)
        {
        }
        
        [HttpPost, Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        [HttpPost]
        [Route("by-publication")]
        public async Task<IActionResult> GetComments(GetCommentsByPublicationQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpDelete("{commentId}"), Authorize]
        public async Task<IActionResult> DeleteCommentById(int commentId)
        {
            var result = await Mediator.Send(new DeleteCommentCommand() { CommentId= commentId });
            return Ok(result);
        }
    }
}
