
namespace Core.Library.Clean.AdditionalService
{
    public interface IEntityDirector<TEntity, TCreate>
        where TEntity : class
        where TCreate : class
    {
        Task<IEnumerable<TEntity>> GetEntitiesAsync(CancellationToken cancellationToken);

        Task<TEntity> GetEntityByIdAsync(string entityId, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> SearchEntitiesAsync(string searchValue, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> SearchEntitiesByForeignIdAsync(string foreignId, CancellationToken cancellationToken);

        Task<long> UpdateEntityByIdAsync(string entityId, TEntity entity, CancellationToken cancellationToken);

        Task<long> UpdateEntitiesAsync(IEnumerable<string> bookIds, IEnumerable<TEntity> entitys, CancellationToken cancellationToken);

        Task<TEntity> CreateEntityAsync(TCreate entity, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> CreateEntitiesAsync(IEnumerable<TCreate> entitys, CancellationToken cancellationToken);

        Task<long> DeleteEntityByIdAsync(string entityId, CancellationToken cancellationToken);

        Task<long> DeleteEntitiesAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> LoadAllEntityForNewDatabase(CancellationToken cancellationToken);
    }
}