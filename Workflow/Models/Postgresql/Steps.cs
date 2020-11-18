using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Workflow.ViewModels;

namespace Workflow.Models.Postgresql
{
    public class Steps
    {
        [Key] public int StepId { get; set; }

        public int? AcceptStepId { get; set; }
        public int? RejectStepId { get; set; }

        [Required] public short StepType { get; set; }
        [Required] public string Name { get; set; }
        [Required] public short Status { get; set; }

        public string Locations { get; set; }

        public int WorkflowId { get; set; }

        [ForeignKey("WorkflowId")] public Workflows Workflow { get; set; }
    }
}