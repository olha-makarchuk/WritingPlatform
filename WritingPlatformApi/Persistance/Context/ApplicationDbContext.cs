using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Genre> Genre {  get; set; }
        public DbSet<Publication> Publication { get; set; }
        public DbSet<Comment> Comment { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /*modelBuilder.Entity<Comment>()
                .HasOne(c => c.ApplicationUser)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserId)
                .OnDelete(DeleteBehavior.NoAction); // or DeleteBehavior.Cascade*/

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Publication)
                .WithMany()
                .HasForeignKey(c => c.PublicationId)
                .OnDelete(DeleteBehavior.NoAction); // or DeleteBehavior.Cascade
        }
    }
}
