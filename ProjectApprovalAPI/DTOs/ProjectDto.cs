namespace ProjectApprovalAPI.DTOs
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AreaId { get; set; }
        public int ProjectTypeId { get; set; }
        public int ApplicantUserId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ApprovalStepDto> Approvals { get; set; }
        public List<ApprovalStepDto> Steps { get; set; }
    }
}
