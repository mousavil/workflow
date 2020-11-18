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
            var workflows = (await _stepRepo.GetByConditionAsync(x => x.WorkflowId == id));

            var output = workflows.Select(item => new
            {
                item.StepId,
                item.Name,
                item.StepType,
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
                StepType = (short)x.StepType,
                WorkflowId = workflowId,
                Locations = x.Locations
            });
            await _stepRepo.AddRangeAsync(steps);
            
            var addedSteps = (await _stepRepo.GetByConditionAsync(x => x.WorkflowId == workflowId)).OrderBy(x=>x.StepId).ToList();
           
            var  updateSteps = new List<Steps>();
            int? accept;
            int? reject;
            foreach (var item in addedSteps)
            {
               
                if (input[addedSteps.IndexOf(item)].AcceptStepId != null)
                    accept = addedSteps[(int) input[addedSteps.IndexOf(item)].AcceptStepId].StepId;
                else
                    accept = null;
                if (input[addedSteps.IndexOf(item)].RejectStepId != null)
                    reject = addedSteps[(int) input[addedSteps.IndexOf(item)].RejectStepId].StepId;
                else
                    reject = null;
                updateSteps.Add(new Steps()
                {
                    StepId = item.StepId,
                    Name = item.Name,
                    Status = 0,
                    StepType = item.StepType,
                    WorkflowId = workflowId,
                    AcceptStepId =accept,
                    RejectStepId = reject,
                    Locations = item.Locations
                    
                });
            }

    
            return Ok(updateSteps);

        }
        
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateAfterCreate([FromBody]List<Steps> input)
        {
            await _stepRepo.UpdateRangeAsync(input);
            return Ok();
        }


        [HttpPut("[action]/{workflowId}")]
        public async Task<IActionResult> UpdateStepAsync([FromRoute] int workflowId,[FromBody] List<StepUpdateViewModel> input)
        {
            var steps = (await _stepRepo.GetByConditionAsync(x => x.WorkflowId == workflowId)).ToList();
            var validateSteps = steps
                .Select(x=>input.Select(y => y.StepId).Contains(x.StepId)
                           && 
                           input.Select(y => y.Name).Contains(x.Name)).Count();
            if (validateSteps != input.Count)
                return BadRequest();


            var output = steps.Join(input, x => x.StepId, y => y.StepId, (x, y) => new Steps()
            {
                StepId       = x.StepId,
                Locations    = y.Locations,
                Name         = y.Name,
                Status       = x.Status,
                StepType       = (short)y.StepType,
                AcceptStepId = y.AcceptStepId,
                RejectStepId = y.RejectStepId,
                WorkflowId   = x.WorkflowId
            });
            
             await _stepRepo.UpdateRangeAsync(output);
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