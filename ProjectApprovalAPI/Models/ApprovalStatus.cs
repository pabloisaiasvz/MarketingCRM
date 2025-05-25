using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApprovalAPI.Models
{
    public class ApprovalStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }
        public virtual ICollection<ProjectProposal> ProjectProposals { get; set; }
        public virtual ICollection<ProjectApprovalStep> ProjectApprovalSteps { get; set; }
    }
}
