using ProjectApprovalAPI.DTOs;
using ProjectApprovalAPI.DTOs.ProjectApprovalAPI.DTOs;
using ProjectApprovalAPI.Models;

namespace ProjectApprovalAPI.Services.Interfaces
{
    public interface IProjectService
    {
        Task<Guid> CreateProposalAsync(ProjectProposalDto dto);
        Task<ProjectDetailsDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProjectListItemDto>> GetAllAsync(string? title = null, int? statusId = null, int? createdById = null, int? approverUserId = null);
        Task<IEnumerable<ProjectListItemDto>> GetFilteredAsync(ProjectFilterDto filter);
        Task<bool> UpdateProposalAsync(Guid id, ProjectProposalDto dto);

        Task<bool> ApproveAsync(Guid projectId, int approverUserId);
        Task<bool> ObserveAsync(Guid projectId, int approverUserId, string observation);
        Task<bool> RejectAsync(Guid projectId, int approverUserId);

        Task<ProjectOptionsDto> GetProjectOptionsAsync();
        Task<DecisionOptionsDto> GetDecisionOptionsAsync(Guid projectId);
        Task<EditProjectOptionsDto> GetEditProjectOptionsAsync(Guid projectId);
        Task<IEnumerable<AreaDto>> GetAreasAsync();
        Task<IEnumerable<ProjectTypeDto>> GetProjectTypesAsync();
        Task<IEnumerable<RoleDto>> GetRolesAsync();
        Task<IEnumerable<ApprovalStatusDto>> GetApprovalStatusesAsync();
        Task<IEnumerable<UserDto>> GetUsersAsync();

    }


}
