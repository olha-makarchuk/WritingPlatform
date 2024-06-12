using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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

        public CreateSortByItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SortByItem> Handle(CreateSortByItemCommand command, CancellationToken cancellationToken)
        {
            var sortByItemm = await _context.SortByItem.FirstOrDefaultAsync(u => u.FieldName == command.FieldName, cancellationToken)
             ?? throw new Exception("SortByItemm alredy exists");

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
