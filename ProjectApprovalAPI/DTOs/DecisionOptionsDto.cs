using ProjectApprovalAPI.DTOs.ProjectApprovalAPI.DTOs;

namespace ProjectApprovalAPI.DTOs
{
    public class DecisionOptionsDto
    {
        public List<ApprovalStatusDto> AvailableStatuses { get; set; }
        public string CurrentStatus { get; set; }
    }
}
