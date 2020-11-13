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
using Type = Workflow.Models.Enum.Type;

namespace Workflow.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleController : ControllerBase
    {
        private readonly IBaseRepository<Steps, WorkflowDbContext> _stepRepo;
        private readonly IBaseRepository<Workflows, WorkflowDbContext> _workflowRepo;
        private readonly IBaseRepository<Rules, WorkflowDbContext> _ruleRepo;

        public RuleController(IBaseRepository<Steps, WorkflowDbContext> stepRepo,
                              IBaseRepository<Workflows, WorkflowDbContext> workflowRepo,
                              IBaseRepository<Rules, WorkflowDbContext> ruleRepo)
        {
            _stepRepo     = stepRepo;
            _workflowRepo = workflowRepo;
            _ruleRepo     = ruleRepo;
        }


        [HttpGet("[action]/{workflowId}")]
        public async Task<IActionResult> GetAll([FromRoute] int workflowId)
        {
            var workflowExists = await _workflowRepo.GetByIdAsync(workflowId) == null;

            if (!workflowExists)
                return BadRequest("Workflow Does Not Exist");

            var steps = (await _stepRepo.GetByConditionAsync(y => y.WorkflowId == workflowId)).Select(z => z.StepId);
            var workflowRules = await _ruleRepo.GetByConditionAsync(x =>
                (x.ReferenceId == workflowId && x.EntityType == (short) EntityType.Workflow)
                ||
                (steps.Contains(x.ReferenceId) && x.EntityType == (short) EntityType.Step));

            var output = workflowRules.Select(item => new
            {
                item.RuleId,
                item.Condition,
                item.Name,
                item.Type,
                item.EntityType,
                item.ReferenceId
            }).ToList();
            return Ok(output);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Add([FromBody] AddRuleViewModel input)
        {
            var ruleExists = await _ruleRepo.GetByConditionAsync(x => x.Type == (short)input.Type && x.ReferenceId == input.ReferenceId) !=null;
            if (ruleExists)
                return BadRequest("Is Exists");
            bool workflowExist = false, stepExist = false;
            switch (input.EntityType)
            {
                case EntityType.Workflow:
                    workflowExist = (await _workflowRepo.GetByIdAsync(input.ReferenceId)) != null;
                    break;
                case EntityType.Step:
                    stepExist = (await _stepRepo.GetByIdAsync(input.ReferenceId)) != null;
                    break;
            }

            if (!workflowExist && !stepExist)
                return BadRequest();
            if (input.Type == Type.ExpireDate)
            {
                try
                {
                    Convert.ToDateTime(input.Condition);
                }
                catch (FormatException ex)
                {
                    return UnprocessableEntity();
                }
            }
            else if (input.Type == Type.StepNumber )
                if(!workflowExist)
                return BadRequest("Workflow Not Found");
                else
                {
                    try
                    {
                        Convert.ToInt16(input.Condition);
                    }
                    catch (FormatException ex)
                    {
                        return UnprocessableEntity();
                    }
                }
            await _ruleRepo.AddAsync(new Rules()
            {
                Name        = input.Name,
                Condition   = input.Condition,
                Type        = (short) input.Type,
                EntityType  = (short) input.EntityType,
                ReferenceId = input.ReferenceId,
            });

            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] RuleUpdateViewModel input)
        {

            var rule = await _ruleRepo.GetByIdAsync(id);
            if (rule == null)
                return NotFound();
            
            if(rule.Type == (short)Type.ExpireDate)
                try
                {
                    Convert.ToDateTime(input.Condition);
                }
                catch (FormatException ex)
                {
                    return UnprocessableEntity();
                }
            else if(rule.Type == (short)Type.StepNumber)
            {
                try
                {
                    Convert.ToInt16(input.Condition);
                }
                catch (FormatException ex)
                {
                    return UnprocessableEntity();
                }
            }
            rule.Name         = input.Name;
            rule.Condition = input.Condition;
            

            await _ruleRepo.UpdateAsync(rule);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var rule = await _ruleRepo.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            await _ruleRepo.DeleteAsync(rule);
            return Ok();
        }
    }
}