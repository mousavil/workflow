using System;
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
    
    public class WorkflowsController : ControllerBase
    {
        private readonly IBaseRepository<Workflows, WorkflowDbContext> _workflowRepo;
        public WorkflowsController(IBaseRepository<Workflows, WorkflowDbContext> workflowRepo)
        {
            _workflowRepo = workflowRepo;
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

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var workflow = await _workflowRepo.GetByIdAsync(id);

            if (workflow == null)
                return NotFound("شهر یافت نشد");

            var output = new
            {
                Name = workflow.Name,
                Description  = workflow.Description,
                Status  = workflow.Status
            };

            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromRoute] WorkflowAddViewModel input)
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
        public async Task<IActionResult> UpdateAddressAsync([FromRoute] int id, [FromBody] WorkflowUpdateViewModel input)
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
            if (workflow == null)
                return NotFound();

            await _workflowRepo.DeleteAsync(workflow);
            return Ok();
        }

    }
}