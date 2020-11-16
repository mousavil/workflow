
using Microsoft.EntityFrameworkCore;
using Workflow.Models.Postgresql;

namespace Workflow.Context
{
    public class WorkflowDbContext : DbContext
    {
        public DbContextOptions<WorkflowDbContext> Options { get; }

        public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options)
            : base(options)
        {
            Options = options;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            base.OnModelCreating(modelBuilder);
            

        }
        public DbSet<Workflows> Workflows { get; set; }
        public DbSet<Rules> Rules{ get; set; }
        public DbSet<Steps> Steps{ get; set; }

    }
}
