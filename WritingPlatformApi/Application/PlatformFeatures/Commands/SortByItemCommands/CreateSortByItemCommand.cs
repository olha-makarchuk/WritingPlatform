using Application.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.PlatformFeatures.Commands.SortByItemCommands
{
    public class CreateSortByItemCommand : IRequest<SortByItem>
    {
        public string ItemName { get; set; } = null!;
        public string FieldName { get; set; } = null!;
    }
    public class CreateSortByItemCommandHandler : IRequestHandler<CreateSortByItemCommand, SortByItem>
    {
        private readonly IApplicationDbContext _context;
        private IApiClientGoogleDrive _client;

        public CreateSortByItemCommandHandler(IApplicationDbContext context, IApiClientGoogleDrive client)
        {
            _context = context;
            _client = client;
        }

        public async Task<SortByItem> Handle(CreateSortByItemCommand command, CancellationToken cancellationToken)
        {
            var sortByItem = new SortByItem
            {
                ItemName = command.ItemName,
                FieldName = command.FieldName
            };

            _context.SortByItem.Add(sortByItem);
            await _context.SaveChangesAsync();

            return sortByItem;
        }
    }
}
