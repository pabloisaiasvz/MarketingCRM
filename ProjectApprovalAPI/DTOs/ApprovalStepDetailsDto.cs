using ProjectApprovalAPI.DTOs.ProjectApprovalAPI.DTOs;

namespace ProjectApprovalAPI.DTOs
{
    public class ApprovalStepDetailsDto
    {
        public long Id { get; set; }
        public int StepOrder { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string? Observations { get; set; }
        public RoleDto ApproverRole { get; set; }
        public ApprovalStatusDto Status { get; set; }
        public UserDto? ApproverUser { get; set; }
    }
}
