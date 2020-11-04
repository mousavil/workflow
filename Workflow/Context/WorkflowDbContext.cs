
using Microsoft.EntityFrameworkCore;

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
        }

    }
}
