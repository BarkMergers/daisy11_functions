using Daisy11Functions.Database.NewWorld.Tables;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Daisy11Functions.Database.NewWorld
{
    public class NewWorldContext : DbContext, INewWorldContext
    {
        public NewWorldContext(DbContextOptions<NewWorldContext> options) : base(options) { }

        public DbSet<Agent> Agent { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Tenant> Tenant { get; set; }

        public async Task<int> ExecuteSqlRawAsync(string sql)
        {
            return await Database.ExecuteSqlRawAsync(sql);
        }

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await Database.BeginTransactionAsync();
        }
    }
}