using Chat.App.Database.Abstraction.Abstracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chat.App.Database.Abstraction;

public class GenericRepository<TEntity>(IDbContextAccessor dbContextAccessor) : IGenericRepository<TEntity>
    where TEntity : class
{
    public IDbContextAccessor Accessor { get; } = dbContextAccessor;

    public async Task<TEntity?> GetByIdAsync(CancellationToken ct = default, params object[] keyValues)
        => await Accessor.Set<TEntity>().FindAsync(keyValues, ct);

    public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct = default)
        => await Accessor.Set<TEntity>().AsNoTracking().ToListAsync(ct);

    public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        => await Accessor.Set<TEntity>().AsNoTracking().Where(predicate).ToListAsync(ct);

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await Accessor.Set<TEntity>().AddAsync(entity, ct);
        return entity;
    }

    public void Update(TEntity entity) => Accessor.Set<TEntity>().Update(entity);

    public void Remove(TEntity entity) => Accessor.Set<TEntity>().Remove(entity);

    public Task<int> CountAsync(CancellationToken ct = default)
        => Accessor.Set<TEntity>().CountAsync(cancellationToken: ct);

    public IQueryable<TEntity> AsQueryable()
        => Accessor.Set<TEntity>().AsQueryable();
}
