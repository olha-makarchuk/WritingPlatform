using Application.Interfaces;
using Application.PlatformFeatures.Commands.CommentCommands;
using Application.PlatformFeatures.Commands.UserAccountCommands;
using Application.PlatformFeatures.Queries.CommentQueries;
using Contracts.Responses;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WritingPlatformApi.Controllers;
using WritingPlatformApi.GraphQL;

namespace Application.PlatformFeatures.GraphQL
{
    public class Mutation: BaseGraphQL
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public Mutation(IApplicationDbContext context, UserManager<ApplicationUser> userManager, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Comment> CreateComment(CreateCommentCommand command)
        {
            var comment = await Mediator.Send(command);
            
            return comment!;
        }

        public async Task<ApplicationUser> GetUserByUserName(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            return user!;
        }

        public async Task<Comment> DeleteComment(DeleteCommentCommand command)
        {
            var comment = await Mediator.Send(command);

            return comment;
        }

        public async Task<ApplicationUser> ChangePersonalInformation(ChangePersonalInformationCommand command)
        {
            var user = await Mediator.Send(command);

            return user;
        }

        public async Task<List<CommentResponse>> GetCommentsByPublicationId(GetCommentsByPublicationQuery command)
        {
            var comments = await Mediator.Send(command);

            return comments;
        }
    }
}
