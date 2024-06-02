using Application.Interfaces;
using Application.PlatformFeatures.Queries.GenreQueries;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.PlatformFeatures.Commands.GenreCommands
{
    public class UpdateGenreCommand : IRequest<Genre>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string FilePath { get; set; } = null!;
    }
    public class UpdateGenreCommandHandler : IRequestHandler<UpdateGenreCommand, Genre>
    {
        private readonly IApplicationDbContext _context;
        private IApiClientGoogleDrive _client;

        public UpdateGenreCommandHandler(IApplicationDbContext context, IApiClientGoogleDrive client)
        {
            _context = context;
            _client = client;
        }

        public async Task<Genre> Handle(UpdateGenreCommand command, CancellationToken cancellationToken)
        {
            var genre = await _context.Genre.Where(a => a.Id == command.Id).FirstOrDefaultAsync(cancellationToken)
                ?? throw new Exception("Genre not found");

            _client.UpdateFile(command.FilePath, "GenreFolder");

            if(command.Name != genre.Name)
            {
                genre.Name = command.Name;
                _context.Genre.Add(genre);
                await _context.SaveChangesAsync();
            }

            return genre;
        }
    }
}
