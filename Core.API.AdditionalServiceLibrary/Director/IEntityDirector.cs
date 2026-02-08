
namespace Core.API.AdditionalServiceLibrary
{
    public interface IEntityDirector<TEntity>
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetEntitiesAsync(CancellationToken cancellationToken);

        Task<TEntity> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken);

        Task<long> UpdateEntityByIdAsync(string entityId, TEntity entity, CancellationToken cancellationToken);

        Task<long> UpdateEntitiesAsync(string searchValue, IEnumerable<TEntity> entitys, CancellationToken cancellationToken);

        Task<TEntity> CreateEntityAsync(TEntity entity, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> CreateEntitiesAsync(IEnumerable<TEntity> entitys, CancellationToken cancellationToken);

        Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken);

        Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> LoadAllEntityForNewDatabase(CancellationToken cancellationToken);
    }
}