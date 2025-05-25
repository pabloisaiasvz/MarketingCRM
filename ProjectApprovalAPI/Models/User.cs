using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApprovalAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public virtual ApproverRole Role { get; set; }

        public virtual ICollection<ProjectProposal> CreatedProjects { get; set; }
        public virtual ICollection<ProjectApprovalStep> ApproverSteps { get; set; }
    }
}
