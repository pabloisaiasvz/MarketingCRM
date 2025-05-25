namespace ProjectApprovalAPI.DTOs
{
    public class ProjectProposalDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int AreaId { get; set; }
        public int TypeId { get; set; }
        public decimal EstimatedAmount { get; set; }
        public int EstimatedDuration { get; set; }
        public int CreateById { get; set; }
    }
}
