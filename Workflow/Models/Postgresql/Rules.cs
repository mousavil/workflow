using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Workflow.Models.Postgresql
{
    public class Rules
    {
        [Key]
        public int StepId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Condition { get; set; }
        public string Description { get; set; }
        public int WorkflowId { get; set; }
        
        
        [ForeignKey("WorkflowId")]
        public Workflows Workflows { get; set; }
    }
}