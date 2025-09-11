using Daisy11Functions.Database.NewWorld.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Daisy11Functions.Database.NewWorld
{
    public interface INewWorldContext : IDisposable
    {
        DbSet<Agent> Agent { get; set; }
        DbSet<Customer> Customer { get; set; }
        DbSet<Tenant> Tenant { get; set; }
        int SaveChanges();
        Task<IDbContextTransaction> BeginTransaction();
        Task<int> ExecuteSqlRawAsync(string sql);
    }
}