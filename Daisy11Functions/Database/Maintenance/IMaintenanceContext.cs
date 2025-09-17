using Daisy11Functions.Database.Archive.Tables;
using Daisy11Functions.Database.Maintenance.Tables;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Daisy11Functions.Database
{
    public interface IMaintenanceContext : IDisposable
    {
        DbSet<Asset> Asset { get; set; }
        DbSet<Account> Account { get; set; }

        int SaveChanges();
        Task<IDbContextTransaction> BeginTransaction();
    }
}
