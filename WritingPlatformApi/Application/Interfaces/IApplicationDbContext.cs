using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Genre> Genre { get; set; }
        DbSet<Publication> Publication { get; set; }
        DbSet<Comment> Comment { get; set; }
        DbSet<SortByItem> SortByItem { get; set; }
        DbSet<UserRewiew> UserRewiew { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
