using System.Linq.Expressions;

namespace Core.API.AdditionalServiceLibrary
{
    public interface IEntityGenericRepository<TEntity>
       where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetEntitiesAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken);

        Task<TEntity> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken);

        Task<long> CreateEntityAsync(TEntity entity, CancellationToken cancellationToken);

        Task<long> CreateEntitiesAsync(IEnumerable<TEntity> entityies, CancellationToken cancellationToken);

        Task<long> UpdateAsync(string entityId, TEntity entity, CancellationToken cancellationToken);

        Task<long> UpdateManyAsync(Expression<Func<TEntity, bool>> predicate, IEnumerable<TEntity> entityies, CancellationToken cancellationToken);

        Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken);

        Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken);
    }
}
