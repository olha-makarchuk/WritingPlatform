using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Commands.RewiewCommand
{
    public class DeleteRewiewCommand : IRequest<UserRewiew>
    {
        public int RewiewId { get; set; }
    }

    public class DeleteRewiewCommandHandler : IRequestHandler<DeleteRewiewCommand, UserRewiew>
    {
        private readonly IApplicationDbContext _context;

        public DeleteRewiewCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserRewiew> Handle(DeleteRewiewCommand command, CancellationToken cancellationToken)
        {
            var rewiew = await _context.UserRewiew.Where(u => u.Id == command.RewiewId).FirstOrDefaultAsync()
             ?? throw new Exception("Rewiew not found");

            var a = await _context.Publication.Where(u => u.Id == rewiew.PublicationId).FirstOrDefaultAsync(cancellationToken);

            int d = 0;

            if(a.CountOfRewiews - 1 != 0)
            {
                d = ((a.Rating * a.CountOfRewiews) - rewiew.Rewiew) / (a.CountOfRewiews - 1);
            }

            a.CountOfRewiews--;
            a.Rating = d;

            _context.UserRewiew.Remove(rewiew);
            _context.Publication.Update(a);
            await _context.SaveChangesAsync(cancellationToken);

            return rewiew;
        }
    }
}
