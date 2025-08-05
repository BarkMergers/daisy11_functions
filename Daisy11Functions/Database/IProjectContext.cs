using Daisy11Functions.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Daisy11Functions.Database
{
    public interface IProjectContext : IDisposable
    {
        DbSet<Role> Role { get; set; }
        //DbSet<Customer> Customer { get; set; }
        int SaveChanges();
    }
}
