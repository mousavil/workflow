using System.ComponentModel.DataAnnotations;
using Workflow.Models.Enum;

namespace Workflow.ViewModels
{
    public class StepUpdateViewModel
    {
        public int StepId { get; set; }
        public int AcceptStepId { get; set; }
        public int RejectStepId { get; set; }

        [Required, EnumDataType(typeof(StepType))]
        public StepType StepType { get; set; }

        public short Type { get; set; }
        public string Name { get; set; }
        public string Locations { get; set; }
    }
}