using Core.Library.ArivuTharavuThalam;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

#nullable disable

namespace Core.API.AdditionalServiceLibrary
{
    /// <summary>
    /// EntityFrameworkGenericRepository implements basic and common data access layer for SQLDb, SQLDBInmemory
    /// EntityFrameworkGenericRepository provides select, insert, update, delete implementation for generic type or tables as dataset
    /// Tell the TABLE and Take the data approach
    /// EntityFrameworkGenericRepository can be used in DIRECTOR, 
    /// BUT idea implementation class of IEntityGenericRepository should provide a wrapper for DIRECTOR USAGE
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityFrameworkGenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        private readonly DbSet<TEntity> dbSet;
        private readonly DbContext dbContext;
        private Type typeOfEntity;

        protected EntityFrameworkGenericRepository(SqlDataBaseDataContext sqlDbContext)
        {
            dbContext = sqlDbContext;

            if (dbContext != null)
            {
                //sqlDbBookContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(3));
                dbSet = dbContext.Set<TEntity>();
                typeOfEntity = typeof(TEntity);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            var result = await dbSet
                            .Where(predicate)
                            .AsNoTracking()
                            .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);

            Logger.LogInformation([$"--> EntityFramework - {result.Count} rows selected from {typeOfEntity.Name}"]);

            return result;
        }

        public virtual async Task<TEntity> FindOneAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            var result = await dbSet
                           .Where(predicate)
                           .AsNoTracking()
                           .FirstOrDefaultAsync(cancellationToken)
                           .ConfigureAwait(false);

            Logger.LogInformation([$"--> EntityFramework - {(result == null ? -1 : 1)} rows SELECTED from {typeOfEntity.Name}"]);

            return result;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await dbSet
                            .AsNoTracking()
                            .ToListAsync(cancellationToken)
                            .ConfigureAwait(false);

            Logger.LogInformation([$"--> EntityFramework - {result.Count} rows SELECTED from {typeOfEntity.Name}"]);

            return result;
        }

        public virtual async Task<long> InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await dbSet
                    .AddAsync(entity, cancellationToken)
                    .ConfigureAwait(false);

            var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            Logger.LogInformation([$"--> EntityFramework - {result} rows INSERTED for {typeOfEntity.Name}"]);

            return result;
        }

        public virtual async Task<long> InsertManyAsync(IEnumerable<TEntity> entityies, CancellationToken cancellationToken)
        {
            await dbSet
                  .AddRangeAsync(entityies, cancellationToken)
                  .ConfigureAwait(false);

            var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            Logger.LogInformation([$"--> EntityFramework - {result} rows INSERTED for {typeOfEntity.Name}"]);

            return result;
        }

        public virtual async Task<long> UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity, CancellationToken cancellationToken)
        {
            var resultToUpdate = await FindOneAsync(predicate, cancellationToken).ConfigureAwait(false);

            if (resultToUpdate == null)
            {
                Logger.LogInformation([$"--> EntityFramework - FAILED to find object for given predicate on Update for {typeOfEntity.Name}"]);

                return default;
            }
            else
            {
                dbSet.Update(entity);

                var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                Logger.LogInformation([$"--> EntityFramework - {result} rows UPDATED for {typeOfEntity.Name}"]);

                return result;
            }
        }

        public virtual async Task<long> UpdateManyAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<TEntity> entityies, CancellationToken cancellationToken)
        {
            var resultsToUpdate = await FindAllAsync(predicate, cancellationToken).ConfigureAwait(false);

            if (resultsToUpdate == null)
            {
                Logger.LogInformation([$"--> EntityFramework - FAILED to find object for given predicate on Update for {typeOfEntity.Name}"]);

                return default;
            }
            else
            {
                dbSet.UpdateRange(entityies);

                var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                Logger.LogInformation([$"-->  EntityFramework - {result} rows UPDATED for {typeOfEntity.Name}"]);

                return result;
            }
        }

        public virtual async Task<long> DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            var resultToDelete = await FindAllAsync(predicate, cancellationToken).ConfigureAwait(false);

            foreach (var entityToDelete in resultToDelete)
            {
                dbContext.Entry(entityToDelete).State = EntityState.Deleted;
                //dbSet.Remove(entityToDelete); //-alternate code for state change
            }

            var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            Logger.LogInformation([$"-->  EntityFramework - {result} rows DELETED for {typeOfEntity.Name}"]);

            return result;
        }

        public virtual async Task<long> DeleteAllAsync(CancellationToken cancellationToken)
        {
            var resultToDelete = await GetAllAsync(cancellationToken).ConfigureAwait(false);

            foreach (var entityToDelete in resultToDelete)
            {
                dbContext.Entry(entityToDelete).State = EntityState.Deleted;
                //dbSet.Remove(entityToDelete); //-alternate code for state change
            }

            var result = await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            Logger.LogInformation([$"-->  EntityFramework - {result} rows DELETED for {typeOfEntity.Name}"]);

            return result;
        }
    }
}
