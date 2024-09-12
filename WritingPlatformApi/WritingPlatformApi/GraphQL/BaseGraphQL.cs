using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace WritingPlatformApi.GraphQL
{
    public abstract class BaseGraphQL
    {
        private IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        protected BaseGraphQL(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected IMediator Mediator => _mediator ??= _serviceProvider.GetRequiredService<IMediator>();
    }
}
