namespace ProjectApprovalAPI.DTOs
{
    namespace ProjectApprovalAPI.DTOs
    {
        public class ProjectDetailsDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = null!;
            public string Description { get; set; } = null!;
            public decimal Amount { get; set; }
            public int Duration { get; set; }

            public AreaDto Area { get; set; } = null!;
            public ProjectTypeDto ProjectType { get; set; } = null!;
            public ApprovalStatusDto Status { get; set; } = null!;

            public List<ApprovalStepDetailsDto> Steps { get; set; } = new();
        }
    }

}
