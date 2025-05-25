using ProjectApprovalAPI.DTOs.ProjectApprovalAPI.DTOs;

namespace ProjectApprovalAPI.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } = null!;
        public RoleDto Role { get; set; } = null!;
    }

}
