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
        public DbSet<Tenant> Tenant { get; set; }




        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await Database.BeginTransactionAsync();
        }


    }
}



// public DbSet<Customer> Customer { get; set; }