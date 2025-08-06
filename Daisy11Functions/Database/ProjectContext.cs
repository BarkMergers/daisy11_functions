using Daisy11Functions.Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Daisy11Functions.Database
{
    public class ProjectContext : DbContext, IProjectContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }
        public DbSet<Agent> Agent { get; set; }
        public DbSet<Tenant> Tenant { get; set; }
        //public DbSet<Customer> Customer { get; set; }
    }
}