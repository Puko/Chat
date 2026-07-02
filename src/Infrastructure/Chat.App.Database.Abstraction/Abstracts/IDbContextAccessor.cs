using Microsoft.EntityFrameworkCore;

namespace Chat.App.Database.Abstraction.Abstracts;

public interface IDbContextAccessor
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}
