using Daisy11Functions.Database.Archive.Tables;
using Daisy11Functions.Database.Maintenance.Tables;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Daisy11Functions.Database
{
    public class MaintenanceContext : DbContext, IMaintenanceContext
    {
        public MaintenanceContext(DbContextOptions<MaintenanceContext> options) : base(options) { }
        public DbSet<Asset> Asset { get; set; }
        public DbSet<Account> Account { get; set; }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await Database.BeginTransactionAsync();
        }
    }
}