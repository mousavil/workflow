using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Workflow.Models.Enum;
using Workflow.Models.Postgresql;

namespace Workflow.ViewModels
{
    public class StepAddViewModel
    {
        [Required] public string Name { get; set; }

        [Required, EnumDataType(typeof(StepType))]
        public StepType StepType { get; set; }

        public int? AcceptStepId { get; set; }
        public int? RejectStepId { get; set; }
        public string Locations { get; set; }
    }
}