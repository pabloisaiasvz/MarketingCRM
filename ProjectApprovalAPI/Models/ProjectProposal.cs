using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApprovalAPI.Models
{
    public class ProjectProposal
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "varchar(max)")]
        public string Description { get; set; }

        [ForeignKey("Area")]
        public int AreaId { get; set; }
        public virtual Area Area { get; set; }

        [ForeignKey("ProjectType")]
        public int TypeId { get; set; }
        public virtual ProjectType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal EstimatedAmount { get; set; }

        [Required]
        public int EstimatedDuration { get; set; }

        [ForeignKey("ApprovalStatus")]
        public int StatusId { get; set; }
        public virtual ApprovalStatus Status { get; set; }

        [Required]
        public DateTime CreateAt { get; set; }

        [ForeignKey("User")]
        public int CreateById { get; set; }
        public virtual User CreateBy { get; set; }

        public virtual ICollection<ProjectApprovalStep> ApprovalSteps { get; set; }
    }
}
