using ProjectApprovalAPI.Enums;

namespace ProjectApprovalAPI.DTOs
{
    public class DecisionDto
    {
        public int ApproverUserId { get; set; }
        public DecisionType Status { get; set; }
        public string? Observation { get; set; }
    }

}
