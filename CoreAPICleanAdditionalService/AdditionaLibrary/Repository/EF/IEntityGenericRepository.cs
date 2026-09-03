namespace Core.Library.Clean.AdditionalService
{
    public interface IEntityGenericRepository<TEntity>
       where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetEntitiesAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken);

        Task<TEntity> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken);

        Task<TEntity> CreateEntityAsync(TEntity entity, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> CreateEntitiesAsync(IEnumerable<TEntity> entityies, CancellationToken cancellationToken);

        Task<long> UpdateAsync(string entityId, TEntity entity, CancellationToken cancellationToken);

        Task<long> UpdateManyAsync(IEnumerable<TEntity> entityies, CancellationToken cancellationToken);

        Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken);

        Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken);
    }
}
