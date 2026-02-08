using Core.Library.ArivuTharavuThalam;
using Microsoft.EntityFrameworkCore;

namespace Core.API.AdditionalServiceLibrary
{
    public class SqlDataBaseDataContext : DbContext
    {
        public SqlDataBaseDataContext(DbContextOptions<SqlDataBaseDataContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        public DbSet<Person> Persons { get; set; }
    }
}
