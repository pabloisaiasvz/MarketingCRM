namespace ProjectApprovalAPI.DTOs
{
    public class ProjectFilterDto
    {
        public string? Title { get; set; }
        public int? StatusId { get; set; }
        public int? ApplicantUserId { get; set; }
        public int? ApprovalUserId { get; set; }
    }

}
