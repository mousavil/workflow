using System.ComponentModel.DataAnnotations;

namespace Workflow.ViewModels
{
    public class StepUpdateViewModel
    {
        
        public int AcceptStepId { get; set; }
        public int RejectStepId { get; set; }
        
        public string Name { get; set; }
        public string Locations { get; set; }
        
    }
}