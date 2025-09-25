using Daisy11Functions.Database.NewWorld.Tables;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Daisy11Functions.Database.NewWorld
{
    public interface INewWorldContext : IDisposable
    {
        DbSet<Agent> Agent { get; set; }
        DbSet<Customer> Customer { get; set; }
        DbSet<Tenant> Tenant { get; set; }
        DbSet<Product> Product { get; set; }
        DbSet<Inventory> Inventory { get; set; }



        int SaveChanges();
        Task<IDbContextTransaction> BeginTransaction();
        Task<int> ExecuteSqlRawAsync(string sql);
        Task<int> ExecuteSqlRawAsync(string sql, params SqlParameter[] paramList);


        public System.Data.Common.DbConnection GetDbConnection();


    }
}