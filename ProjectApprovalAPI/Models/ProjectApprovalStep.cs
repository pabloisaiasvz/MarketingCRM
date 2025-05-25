using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApprovalAPI.Models
{
    public class ProjectApprovalStep
    {
        [Key]
        public long Id { get; set; }

        public Guid ProjectProposalId { get; set; }

        public int? ApproverUserId { get; set; }

        public int ApproverRoleId { get; set; }

        public int StatusId { get; set; }

        public int StepOrder { get; set; }

        public DateTime? DecisionDate { get; set; }

        [Column(TypeName = "varchar(max)")]
        public string? Observations { get; set; }

        [ForeignKey("ProjectProposalId")]
        public ProjectProposal ProjectProposal { get; set; }

        [ForeignKey("ApproverUserId")]
        public User ApproverUser { get; set; }

        [ForeignKey("ApproverRoleId")]
        public ApproverRole ApproverRole { get; set; }

        [ForeignKey("StatusId")]
        public ApprovalStatus Status { get; set; }
    }
}
