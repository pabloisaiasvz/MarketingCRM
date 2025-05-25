namespace ProjectApprovalAPI.DTOs
{
    public class ProjectProposalUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AreaId { get; set; }
        public int ProjectTypeId { get; set; }
    }

}
