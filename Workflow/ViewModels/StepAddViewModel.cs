using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Workflow.Models.Postgresql;

namespace Workflow.ViewModels
{
    public class StepAddViewModel
    {
        [Required]
        public string Name { get; set; }
        public int? AcceptStepId { get; set; }     
        public int? RejectStepId { get; set; }
        
    }
}