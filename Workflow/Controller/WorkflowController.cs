using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    
    public class WorkflowsController : ControllerBase
    {
        private readonly IBaseRepository<Workflows, WorkflowDbContext> _workflowRepo;
        private readonly IBaseRepository<Rules, WorkflowDbContext> _ruleRepo;
        public WorkflowsController(IBaseRepository<Workflows, WorkflowDbContext> workflowRepo, IBaseRepository<Rules, WorkflowDbContext> ruleRepo)
        {
            _workflowRepo  = workflowRepo;
            _ruleRepo = ruleRepo;
        }


        [HttpGet("[action]/{userId}")]
        public async Task<IActionResult> GetAll([FromRoute] int userId)
        {
            var workflows = await _workflowRepo.GetByConditionAsync(x=>x.CreatorId == userId);
            
            var output = workflows.Select(item => new 
            {
                WorkflowId  = item.Id,
                Name        = item.Name,
                Status      = item.Status,
                Description = item.Description,
                CreateDate = item.CreationDate,

            }).ToList();
            return Ok(output);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetById([FromBody] GetWorkflow input)
        {
            
            var workflow = await _workflowRepo.GetByIdAsync(input.Id);

            if (workflow == null)
                return NotFound("گردش کار یافت نشد");

            if (workflow.CreatorId != input.UserId)
                return Unauthorized();
            
            var output = new
            {
                 workflow.Name,
                 workflow.Description,
                 workflow.Status
            };

            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] WorkflowAddViewModel input)
        {
            var isDuplicate = (await _workflowRepo.GetByConditionAsync(x => x.Name == input.Name)).Any();
            if( isDuplicate)
                return BadRequest("Duplicate");
            
            await _workflowRepo.AddAsync(new Workflows()
            {
                Name = input.Name,
                CreationDate = DateTime.Now,
                Status = 0,
                CreatorId = input.CreatorId,
                Description = input.Description,
                
            });
            var id = (await _workflowRepo.GetByConditionAsync(x => x.Name == input.Name)).First().Id;
            return Ok(new {WorkflowId = id});
        }
        
           [HttpPut("[action]/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] WorkflowUpdateViewModel input)
        {
            var workflow = await _workflowRepo.GetByIdAsync(id);
            if (workflow == null)
                return NotFound();

            if ((await _workflowRepo.GetByConditionAsync(x => x.Name == input.Name)).Any())
                return BadRequest("Name Exists");
            
            workflow.Name = input.Name;
            workflow.Description = input.Description;

            await _workflowRepo.UpdateAsync(workflow);
            return Ok();
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            var workflow = await _workflowRepo.GetByIdAsync(id);
            
            var rule = (await _ruleRepo.GetByConditionAsync(x =>
                x.EntityType == (short) EntityType.Workflow && x.ReferenceId == id)).FirstOrDefault(); 
            if (workflow == null)
                return NotFound();
            if (rule != null)
                await _ruleRepo.DeleteAsync(rule);
            
            await _workflowRepo.DeleteAsync(workflow);
            return Ok();
        }

    }
}