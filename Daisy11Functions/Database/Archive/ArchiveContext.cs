using Daisy11Functions.Database.Archive.Tables;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Daisy11Functions.Database
{
    public class ArchiveContext : DbContext, IArchiveContext
    {
        public ArchiveContext(DbContextOptions<ArchiveContext> options) : base(options) { }
        public DbSet<Log> Log { get; set; }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await Database.BeginTransactionAsync();
        }


    }
}