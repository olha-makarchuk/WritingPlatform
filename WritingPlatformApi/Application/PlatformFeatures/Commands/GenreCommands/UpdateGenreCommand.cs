using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.PlatformFeatures.Commands.GenreCommands
{
    public class UpdateGenreCommand : IRequest<Genre>
    {
        [Required(ErrorMessage = "Id is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(256, ErrorMessage = "Name cannot be longer than 256 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "FilePath is required.")]
        public IFormFile FilePath { get; set; } = null!;
    }
    public class UpdateGenreCommandHandler : IRequestHandler<UpdateGenreCommand, Genre>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBlobStorage _storage;

        public UpdateGenreCommandHandler(IApplicationDbContext context, IBlobStorage storage)
        {
            _context = context;
            _storage = storage;
        }

        public async Task<Genre> Handle(UpdateGenreCommand command, CancellationToken cancellationToken)
        {
            var genre = await _context.Genre.FirstOrDefaultAsync(a => a.Id == command.Id, cancellationToken)
                ?? throw new NotFoundException("Genre not found");

            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var genreFileName = $"{Guid.NewGuid()}_{timestamp}_{command.FilePath.FileName}";

            await using (var genreStream = command.FilePath.OpenReadStream())
            {
                await _storage.PutContextAsync(genreFileName, genreStream);
            }

            await _storage.DeleteAsync(genre.FileKey);

            genre.FileKey = genreFileName;
            genre.Name = command.Name;

            await _context.SaveChangesAsync(cancellationToken);

            return genre;
        }
    }
}
