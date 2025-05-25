namespace ProjectApprovalAPI.DTOs
{
    public class ApprovalDecisionDto
    {
        public string Decision { get; set; }
        public int ApproverUserId { get; set; }
        public string? Observation { get; set; }
    }

}
