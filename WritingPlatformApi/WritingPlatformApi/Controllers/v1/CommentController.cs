using Application.PlatformFeatures.Commands.CommentCommands;
using Application.PlatformFeatures.Queries.CommentQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WritingPlatformApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CommentController : BaseApiController
    {
        public CommentController(IMediator mediator) : base(mediator)
        {
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentCommand request)
        {
            var a = await Mediator.Send(request);
            return Ok(a);
        }

        [HttpPost]
        [Route("by-publication")]
        public async Task<IActionResult> GetComments(GetCommentsByPublicationQuery query)
        {
            var a = await Mediator.Send(query);
            return Ok(a);
        }

        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteCommentById(int commentId)
        {
            var a = await Mediator.Send(new DeleteCommentCommand() { CommentId= commentId });
            return Ok(a);
        }
    }
}
