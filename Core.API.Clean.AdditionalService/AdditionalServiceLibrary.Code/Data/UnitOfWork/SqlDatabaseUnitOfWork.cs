using Core.Library.ArivuTharavuThalam;
using Microsoft.EntityFrameworkCore;

namespace Core.API.AdditionalServiceLibrary
{
    public class SqlDatabaseUnitOfWork : IUnitOfWork
    {
        private readonly DbContext sqlDbContext;

        public IEntityGenericRepository<Book> BookRepository { get; }

        public IEntityGenericRepository<Person> PersonRepository { get; }

        public SqlDatabaseUnitOfWork(SqlDataBaseDataContext sqlDbContext,
            IEntityGenericRepository<Book> bookRepository,
            IEntityGenericRepository<Person> personRepository)
        {
            this.sqlDbContext = sqlDbContext;
            BookRepository = bookRepository;
            PersonRepository = personRepository;
        }

        public virtual Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return sqlDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
