using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Workflow.Models.Postgresql
{
    public class Rules
    {
        [Key]
        public int RuleId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Condition { get; set; }
        public string Description { get; set; }
        public short Type { get; set; }
        public short EntityType { get; set; }
        public int ReferenceId { get; set; }
    }
}