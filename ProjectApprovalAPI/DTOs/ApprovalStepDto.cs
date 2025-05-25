using ProjectApprovalAPI.DTOs.ProjectApprovalAPI.DTOs;

namespace ProjectApprovalAPI.DTOs
{
    public class ApprovalStepDto
    {
        public long Id { get; set; }
        public int StepOrder { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string? Observations { get; set; }

        public UserDto ApproverUser { get; set; } = null!;
        public RoleDto ApproverRole { get; set; } = null!;
        public ApprovalStatusDto Status { get; set; } = null!;
        public Guid ProjectProposalId { get; internal set; }
        public int ApproverUserId { get; internal set; }
    }
}

