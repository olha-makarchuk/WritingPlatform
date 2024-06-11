using Microsoft.EntityFrameworkCore;

namespace WritingPlatformApi.Core.Tests
{
    public class DbContextDecorator<T> where T : DbContext
    {
        private readonly DbContextOptions<T> _options;

        private T CreateDbContextInstance()
        {
            return (T)Activator.CreateInstance(typeof(T), _options)!;
        }

        public DbContextDecorator(DbContextOptions<T> options)
        {
            _options = options;
        }

        public void AddAndSave<TEntity>(TEntity entityToSave) where TEntity : class
            => Using(CreateDbContextInstance(), context =>
            {
                context.Add(entityToSave);
                context.SaveChanges();
            });

        public void AddAndSaveRange<TEntity>(IEnumerable<TEntity> entitiesToSave) where TEntity : class
            => Using(CreateDbContextInstance(), context =>
            {
                context.AddRange(entitiesToSave);
                context.SaveChanges();
            });

        public void Assert(Action<T> assert)
            => Using(CreateDbContextInstance(), assert);

        private static void Using<TDisposable>(TDisposable disposable, Action<TDisposable> action) where TDisposable : IDisposable
        {
            using (disposable)
                action(disposable);
        }
    }
}
