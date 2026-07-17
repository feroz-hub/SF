using System.Linq.Expressions;
using System.Threading;
using Zentra.Domain;

namespace Zentra.DomainServices;

public interface IRepository<TEntity> : IDisposable where TEntity : BaseEntity
{
    Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task InsertAsync(IList<TEntity> entityList, CancellationToken cancellationToken = default);

    Task UpdateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity, string[] affectedProperties);
    Task UpdateAsync(IList<TEntity> entityList);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(TEntity entity);
    Task DeleteAsync(IList<TEntity> entityList);

    Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, object>>[] includes = null, CancellationToken cancellationToken = default);
    Task<IQueryable<TEntity>> GetAllForQueryAsync();
    Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IList<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, object>>[] includes = null,
        CancellationToken cancellationToken = default);

    Task<IList<TType>> GetAsync<TType>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TType>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Expression<Func<TEntity, object>>[] includes = null,
        CancellationToken cancellationToken = default) where TType : class;

    Task<IList<TType>> GetWithSoftDeleteAsync<TType>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TType>> select = null,
        CancellationToken cancellationToken = default) where TType : class;

    Task<bool> ActiveRecordExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    Task<bool> DuplicateExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

    Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default);
}
