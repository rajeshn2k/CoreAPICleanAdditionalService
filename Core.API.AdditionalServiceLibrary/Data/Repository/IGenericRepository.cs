using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Library.ArivuTharavuThalam
{
    /// <summary>
    /// IGenericRepository DEFINES Naming Convention for SQL DB select, insert, update, delete
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal interface IGenericRepository<TEntity>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        Task<long> InsertAsync(TEntity entity, CancellationToken cancellationToken);

        Task<long> InsertManyAsync(IEnumerable<TEntity> entityies, CancellationToken cancellationToken);

        Task<long> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity, CancellationToken cancellationToken);

        Task<long> UpdateManyAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<TEntity> entityies, CancellationToken cancellationToken);

        Task<long> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        Task<long> DeleteAllAsync(CancellationToken cancellationToken);
    }
}