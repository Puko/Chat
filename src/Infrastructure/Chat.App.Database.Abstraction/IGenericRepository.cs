using Chat.App.Database.Abstraction.Abstracts;
using System.Linq.Expressions;

namespace Chat.App.Database.Abstraction;

public interface IGenericRepository<TEntity> where TEntity : class
{
    IDbContextAccessor Accessor { get; }
    Task<TEntity?> GetByIdAsync(CancellationToken ct = default, params object[] keyValues);
    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken ct = default);
    Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken ct = default);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task<int> CountAsync(CancellationToken ct = default);
    IQueryable<TEntity> AsQueryable();
}
