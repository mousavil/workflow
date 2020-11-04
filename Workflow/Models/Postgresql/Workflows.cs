using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Workflow.Models.Postgresql
{
    public class Workflows
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }   
        public int CreatorId { get; set; }   
        public string RejectionReason { get; set; }   
        public DateTime CreationDate { get; set; }   
        public short Status { get; set; }
        
        public List<Rules> Rules;
        
        public List<Steps> Steps;
        
    }
}