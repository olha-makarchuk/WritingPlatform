using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Commands.SortByItemCommands
{
    public class CreateSortByItemCommand : IRequest<SortByItem>
    {
        [Required(ErrorMessage = "ItemName is required.")]
        [MaxLength(256, ErrorMessage = "ItemName cannot be longer than 256 characters.")]
        public string ItemName { get; set; } = null!;

        [Required(ErrorMessage = "FieldName is required.")]
        [MaxLength(256, ErrorMessage = "FieldName cannot be longer than 256 characters.")]
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
            var existingSortByItem = await _context.SortByItem
                .FirstOrDefaultAsync(u => u.FieldName == command.FieldName, cancellationToken);

            if (existingSortByItem != null)
            {
                throw new ArgumentException("SortByItem already exists");
            }

            var sortByItem = new SortByItem
            {
                ItemName = command.ItemName,
                FieldName = command.FieldName
            };

            _context.SortByItem.Add(sortByItem);
            await _context.SaveChangesAsync(cancellationToken);

            return sortByItem;
        }
    }
}
