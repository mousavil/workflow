using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Workflow.Context;
using Workflow.Interfaces;
using Workflow.Models.Postgresql;
using Workflow.ViewModels;

namespace Workflow.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class StepController : ControllerBase
    {
        private readonly IBaseRepository<Steps, WorkflowDbContext> _stepRepo;
        private readonly IBaseRepository<Workflows, WorkflowDbContext> _workflowRepo;
        public StepController(IBaseRepository<Steps, WorkflowDbContext> stepRepo, IBaseRepository<Workflows, WorkflowDbContext> workflowRepo)
        {
            _stepRepo         = stepRepo;
            _workflowRepo = workflowRepo;
        }


        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetByWorkflowId([FromRoute] int id)
        {
            var workflows = await _stepRepo.GetByConditionAsync(x=>x.WorkflowId == id);
            
            var output = workflows.Select(item => new 
            {
                 item.StepId,
                 item.Name,
                 item.Status,
                 item.AcceptStepId,
                 item.RejectStepId,
                

            }).ToList();
            return Ok(output);
        }
        

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] List<StepAddViewModel> input)
        {
            var isDuplicate = (await _stepRepo.GetAllAsync()).Select(item => input.Select(x=>x.WorkflowId).Contains(item.WorkflowId) 
                                                                             &&
                                                                             input.Select(x=>x.Name).Contains(item.Name)).Any();
            if( isDuplicate)
                return BadRequest("Duplicate");
            var workflowIsExists =  (await _workflowRepo.GetAllAsync()).Where(item => input.Select(x=>x.WorkflowId).Contains(item.Id)); 
            
            
            
            // await _stepRepo.AddRangeAsync(new List<Steps>()
            // {
            //     Name = input.Name,
            //     Status = 0,
            //     WorkflowId = input.WorkflowId,
            //     
            // });
            // var id = (await _workflowRepo.GetByConditionAsync(x => x.Name == input.Name)).First().Id;
            return Ok(/*new {WorkflowId = id}*/);
        }
        
           [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateStepAsync([FromRoute] int id, [FromBody] StepUpdateViewModel input)
        {
            var workflow = await _stepRepo.GetByIdAsync(id);
            if (workflow == null)
                return NotFound();

            if ((await _stepRepo.GetByConditionAsync(x => x.Name == input.Name && x.WorkflowId == workflow.WorkflowId)).Any())
                return BadRequest("Name Exists");
            
            // var isExists = (await _stepRepo.GetAllAsync()).Select(item => input.Select(x=>x.WorkflowId).Contains(item.WorkflowId) 
            //                                                                  &&
            //                                                                  input.Select(x=>x.Name).Contains(item.Name)).Any();
            workflow.Name = input.Name;
            workflow.AcceptStepId = input.AcceptStepId;
            workflow.RejectStepId = input.RejectStepId;

            // await _workflowRepo.UpdateAsync(workflow);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            var workflow = await _workflowRepo.GetByIdAsync(id);
            if (workflow == null)
                return NotFound();

            await _workflowRepo.DeleteAsync(workflow);
            return Ok();
        }

    }
}