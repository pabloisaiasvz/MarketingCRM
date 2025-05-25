using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectApprovalAPI.Models
{
    public class ApprovalRule
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxAmount { get; set; }

        [ForeignKey("Area")]
        public int? AreaId { get; set; }
        public virtual Area Area { get; set; }

        [ForeignKey("ProjectType")]
        public int? TypeId { get; set; }
        public virtual ProjectType Type { get; set; }

        [Required]
        public int StepOrder { get; set; }

        [ForeignKey("ApproverRole")]
        public int ApproverRoleId { get; set; }
        public virtual ApproverRole ApproverRole { get; set; }
    }
}
