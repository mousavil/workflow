using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Workflow.Models.Postgresql
{
    public class Steps
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public short Status { get; set; }
        public int WorkflowId { get; set; }
        
        [ForeignKey("WorkflowId")]
        public Workflows Workflow { get; set; }
    }
}