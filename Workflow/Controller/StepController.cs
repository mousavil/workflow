using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Workflow.Context;
using Workflow.Interfaces;
using Workflow.Models.Enum;
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
        private readonly IBaseRepository<Rules, WorkflowDbContext> _ruleRepo;

        public StepController(IBaseRepository<Steps, WorkflowDbContext> stepRepo,
                              IBaseRepository<Workflows, WorkflowDbContext> workflowRepo, IBaseRepository<Rules, WorkflowDbContext> ruleRepo)
        {
            _stepRepo      = stepRepo;
            _workflowRepo  = workflowRepo;
            _ruleRepo = ruleRepo;
        }


        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetByWorkflowId([FromRoute] int id)
        {
            var workflows = await _stepRepo.GetByConditionAsync(x => x.WorkflowId == id);

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

        [HttpPost("[action]/{workflowId}")]
        public async Task<IActionResult> Add([FromRoute] int workflowId, [FromBody] List<StepAddViewModel> input)
        {
            var isDuplicate = (await _stepRepo.GetAllAsync()).Select(x => x.WorkflowId == workflowId
                                                                          &&
                                                                          input.Select(y => y.Name).Contains(x.Name))
                .Any();
            if (isDuplicate)
                return BadRequest("Duplicate");

            var workflowIsExists = (await _workflowRepo.GetByIdAsync(workflowId));

            if (workflowIsExists == null)
                return BadRequest("Workflow Does Not Exist");

            var steps = input.Select(x => new Steps()
            {
                Name       = x.Name,
                Status     = 0,
                WorkflowId = workflowId,
            });
            await _stepRepo.AddRangeAsync(steps);
            
            var addedSteps = (await _stepRepo.GetByConditionAsync(x => x.WorkflowId == workflowId)).OrderBy(x=>x.StepId).ToList();
           
            var updateSteps = new List<Steps>();
            
            foreach (var item in addedSteps)
            {
                updateSteps.Add(new Steps()
                {
                    StepId = item.StepId,
                    Name = item.Name,
                    Status = 0,
                    WorkflowId = workflowId,
                    AcceptStepId = addedSteps[input[addedSteps.IndexOf(item)].AcceptStepId].AcceptStepId,
                    RejectStepId = addedSteps[input[addedSteps.IndexOf(item)].RejectStepId].RejectStepId
                });
            }

            await _stepRepo.UpdateRangeAsync(updateSteps);
            return Ok(updateSteps);

        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateStepAsync([FromRoute] int id, [FromBody] StepUpdateViewModel input)
        {
            var step = await _stepRepo.GetByIdAsync(id);
            if (step == null)
                return NotFound();

            if ((await _stepRepo.GetByConditionAsync(x => x.Name == input.Name && x.WorkflowId == step.WorkflowId))
                .Any())
                return BadRequest("Name Exists");

            var isExists = (await _stepRepo.GetByIdAsync(input.AcceptStepId))!=null &&
                           (await _stepRepo.GetByIdAsync(input.RejectStepId))!=null;
            if (!isExists)
                return BadRequest("Accept Or Reject Does Not Exist");
            
            step.Name         = input.Name;
            step.AcceptStepId = input.AcceptStepId;
            step.RejectStepId = input.RejectStepId;

             await _stepRepo.UpdateAsync(step);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var step = await _stepRepo.GetByIdAsync(id);
            
            var rule = (await _ruleRepo.GetByConditionAsync(x =>
                x.EntityType == (short) EntityType.Step && x.ReferenceId == id)).FirstOrDefault(); 
            if (step == null)
                return NotFound();
            
            if (rule != null)
                await _ruleRepo.DeleteAsync(rule);
            
            await _stepRepo.DeleteAsync(step);
            return Ok();
        }
    }
}