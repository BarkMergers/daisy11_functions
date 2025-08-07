using Daisy11Functions.Database.Archive.Tables;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Daisy11Functions.Database
{
    public interface IArchiveContext : IDisposable
    {
        DbSet<Log> Log { get; set; }
        int SaveChanges();
        Task<IDbContextTransaction> BeginTransaction();
    }
}
