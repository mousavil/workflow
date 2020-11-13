using System.ComponentModel.DataAnnotations;
using Workflow.Models.Enum;
using Workflow.Models.Postgresql;

namespace Workflow.ViewModels
{
    public class AddRuleViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Condition { get; set; }
        [Required,EnumDataType(typeof(Type))]
        public Type Type { get; set; }
        [Required,EnumDataType(typeof(EntityType))]
        public EntityType EntityType { get; set; }
        [Required]
        public int ReferenceId { get; set; }
    }
}