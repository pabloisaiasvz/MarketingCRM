using Microsoft.AspNetCore.Mvc;
using ProjectApprovalAPI.DTOs;
using ProjectApprovalAPI.Services.Interfaces;
using System;
using System.Threading.Tasks;
using ProjectApprovalAPI.Enums;

namespace ProjectApprovalAPI.Controllers
{
    [ApiController]
    [Route("Api")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // ----------- Información -----------

        [HttpGet("Area")]
        [Tags("Información")]
        public async Task<IActionResult> GetAreas() =>
            Ok(await _projectService.GetAreasAsync());

        [HttpGet("ProjectType")]
        [Tags("Información")]
        public async Task<IActionResult> GetProjectTypes() =>
            Ok(await _projectService.GetProjectTypesAsync());

        [HttpGet("Role")]
        [Tags("Información")]
        public async Task<IActionResult> GetRoles() =>
            Ok(await _projectService.GetRolesAsync());

        [HttpGet("ApprovalStatus")]
        [Tags("Información")]
        public async Task<IActionResult> GetApprovalStatuses() =>
            Ok(await _projectService.GetApprovalStatusesAsync());

        [HttpGet("User")]
        [Tags("Información")]
        public async Task<IActionResult> GetUsers() =>
            Ok(await _projectService.GetUsersAsync());

        // ----------- Proyectos -----------

        [HttpPost("/Api/Project")]
        [Tags("Proyectos")]
        public async Task<IActionResult> Create([FromBody] ProjectProposalDto dto)
        {
            var newId = await _projectService.CreateProposalAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        [HttpGet("/Api/Project")]
        [Tags("Proyectos")]
        public async Task<IActionResult> GetAll([FromQuery] string? title, [FromQuery] int? statusId, [FromQuery] int? createdById, [FromQuery] int? approverUserId)
        {
            var result = await _projectService.GetAllAsync(title, statusId, createdById, approverUserId);
            return Ok(result);
        }

        [HttpGet("/Api/Project/{id:guid}")]
        [Tags("Proyectos")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var proposal = await _projectService.GetByIdAsync(id);
            if (proposal == null) return NotFound();
            return Ok(proposal);
        }

        [HttpPut("/Api/Project/{id:guid}")]
        [Tags("Proyectos")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProjectProposalDto dto)
        {
            var updated = await _projectService.UpdateProposalAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpPost("Project/{id:guid}/Decision")]
        [Tags("Proyectos")]
        public async Task<IActionResult> Decision(Guid id, [FromBody] DecisionDto dto)
        {
            if (!Enum.IsDefined(typeof(DecisionType), dto.Status))
                return BadRequest("Estado inválido.");

            var decision = (DecisionType)dto.Status;

            bool success = decision switch
            {
                DecisionType.Approve => await _projectService.ApproveAsync(id, dto.ApproverUserId),
                DecisionType.Observe => await _projectService.ObserveAsync(id, dto.ApproverUserId, dto.Observation ?? ""),
                DecisionType.Reject => await _projectService.RejectAsync(id, dto.ApproverUserId),
                _ => false
            };

            if (!success) return BadRequest("No se pudo completar la acción.");
            return NoContent();
        }


        [HttpGet("Project/Options")]
        [Tags("Proyectos")]
        public async Task<IActionResult> GetCreationOptions() =>
            Ok(await _projectService.GetProjectOptionsAsync());

        [HttpGet("Project/{id:guid}/Options")]
        [Tags("Proyectos")]
        public async Task<IActionResult> GetEditOptions(Guid id) =>
            Ok(await _projectService.GetEditProjectOptionsAsync(id));

        [HttpGet("Project/{id:guid}/Decision-Options")]
        [Tags("Proyectos")]
        public async Task<IActionResult> GetDecisionOptions(Guid id) =>
            Ok(await _projectService.GetDecisionOptionsAsync(id));
    }

}
