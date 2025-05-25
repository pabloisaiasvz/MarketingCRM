namespace ProjectApprovalAPI.DTOs
{
    namespace ProjectApprovalAPI.DTOs
    {
        public class ProjectListItemDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public int StatusId { get; set; }
            public string StatusName { get; set; }
            public DateTime CreatedAt { get; set; }
            public string ApplicantUserName { get; set; }
        }
    }

}
