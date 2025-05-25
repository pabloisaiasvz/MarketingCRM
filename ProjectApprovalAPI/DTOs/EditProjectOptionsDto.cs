using ProjectApprovalAPI.DTOs.ProjectApprovalAPI.DTOs;
namespace ProjectApprovalAPI.DTOs
{
    public class EditProjectOptionsDto
    {
        public ProjectDto Project { get; set; }
        public List<AreaDto> Areas { get; set; }
        public List<ProjectTypeDto> ProjectTypes { get; set; }
        public List<RoleDto> Roles { get; set; }
        public List<ApprovalStatusDto> ApprovalStatuses { get; set; }
        public List<UserDto> Users { get; set; }
    }
}
