/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using HCL.CS.Domain;
using HCL.CS.DomainServices;

namespace HCL.CS.Infrastructure.Data;

internal class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IApplicationDbContext context;

    private bool disposed;
    private DbSet<TEntity> entities;

    public BaseRepository(IApplicationDbContext context)
    {
        this.context = context;
        entities = context.Set<TEntity>();
    }

    public virtual Task InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.IsDeleted = false;
        entities.Add(entity);
        return Task.CompletedTask;
    }

    public virtual Task InsertAsync(IList<TEntity> entityList, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entityList)
        {
            entity.IsDeleted = false;
            entities.Add(entity);
        }

        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        entities.Update(entity);
        context.SetRowVersionStatus(entity, entity.RowVersion);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(TEntity entity, string[] affectedProperties)
    {
        entities.Attach(entity);
        foreach (var property in affectedProperties) context.SetPropertyModifiedStatus(entity, property);
        return Task.CompletedTask;
    }

    public virtual Task UpdateAsync(IList<TEntity> entityList)
    {
        foreach (var entity in entityList)
        {
            entities.Update(entity);
            context.SetRowVersionStatus(entity, entity.RowVersion);
        }

        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await entities.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null) entities.Remove(entity);
    }

    public virtual Task DeleteAsync(TEntity entity)
    {
        context.SetRowVersionStatus(entity, entity.RowVersion);
        entities.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(IList<TEntity> entityList)
    {
        foreach (var entity in entityList)
        {
            context.SetRowVersionStatus(entity, entity.RowVersion);
            entities.Remove(entity);
        }

        return Task.CompletedTask;
    }

    public virtual async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, object>>[] includes = null, CancellationToken cancellationToken = default)
    {
        var includeList = includes ?? Array.Empty<Expression<Func<TEntity, object>>>();
        IQueryable<TEntity> query = entities;
        if (includeList.Length > 0)
        {
            foreach (var include in includeList) query = query.Include(include);

            if (includeList.Length > 1) query = query.AsSplitQuery();
        }

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual Task<IQueryable<TEntity>> GetAllForQueryAsync()
    {
        return Task.FromResult(entities.AsNoTracking().AsQueryable());
    }

    public virtual async Task<TEntity> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await entities.FindAsync(new object[] { id }, cancellationToken);
        if (result != null) return result;

        return null;
    }

    public virtual async Task<IList<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, object>>[] includes = null,
        CancellationToken cancellationToken = default)
    {
        if (filter != null)
        {
            var includeList = includes ?? Array.Empty<Expression<Func<TEntity, object>>>();
            IQueryable<TEntity> query = entities;

            if (includeList.Length > 0)
            {
                foreach (var include in includeList) query = query.Include(include);

                if (includeList.Length > 1) query = query.AsSplitQuery();
            }

            query = query.Where(filter);
            return await query.ToListAsync(cancellationToken);
        }

        return null;
    }

    public virtual async Task<IList<TType>> GetWithSoftDeleteAsync<TType>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TType>> select = null,
        CancellationToken cancellationToken = default) where TType : class
    {
        if (filter != null)
        {
            var query = entities.IgnoreQueryFilters();
            query = query.Where(filter);
            if (select != null) return await query.Select(select).ToListAsync(cancellationToken);

            return (IList<TType>)await query.ToListAsync(cancellationToken);
        }

        return null;
    }

    //example: var users = unitOfWork.UserRepository.Get(
    //filter: u => u.RoleId == 2 & u.DepartmentId == 1,
    //orderBy: u => u.UserName,
    //selector: u => u.Select(m => new { m.UserName, m.RoleId }),
    //includeProperties: "Roles,Departments");
    public virtual async Task<IList<TType>> GetAsync<TType>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TType>> select = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Expression<Func<TEntity, object>>[] includes = null,
        CancellationToken cancellationToken = default) where TType : class
    {
        var includeList = includes ?? Array.Empty<Expression<Func<TEntity, object>>>();
        IQueryable<TEntity> query = entities;
        if (filter != null) query = query.Where(filter);

        if (includeList.Length > 0)
        {
            foreach (var include in includeList) query = query.Include(include);

            if (includeList.Length > 1) query = query.AsSplitQuery();
        }

        if (orderBy != null)
        {
            if (select != null) return await orderBy(query).Select(select).ToListAsync(cancellationToken);

            return (IList<TType>)await orderBy(query).ToListAsync(cancellationToken);
        }

        if (select != null) return await query.Select(select).ToListAsync(cancellationToken);

        return (IList<TType>)await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> ActiveRecordExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        if (filter != null)
        {
            IQueryable<TEntity> query = entities;
            query = query.Where(filter);
            return await query.AnyAsync(cancellationToken);
        }

        return false;
    }

    public virtual async Task<bool> DuplicateExistsAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        if (filter != null)
        {
            var query = entities.IgnoreQueryFilters();
            query = query.Where(filter);
            return await query.AnyAsync(cancellationToken);
        }

        return false;
    }

    public virtual async Task<FrameworkResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<FrameworkResult> SaveChangesWithHardDeleteAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesWithHardDeleteAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
            if (disposing)
                if (entities != null)
                    entities = null;

        disposed = true;
    }
}
