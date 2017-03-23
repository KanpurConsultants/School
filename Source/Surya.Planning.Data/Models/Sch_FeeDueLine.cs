using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_FeeDueLine : EntityBase, IHistoryLog
    {
        public Sch_FeeDueLine()
        {

        }

        [Key]        
        public int FeeDueLineId { get; set; }

        [Display(Name = "Purchase Order")]
        [ForeignKey("FeeDueHeader")]
        public int FeeDueHeaderId { get; set; }
        public virtual Sch_FeeDueHeader  FeeDueHeader { get; set; }

        [ForeignKey("Admission"), Display(Name = "Admission")]
        public int AdmissionId { get; set; }
        public virtual Sch_Admission Admission { get; set; }


        [Display(Name = "Fee")]
        [ForeignKey("Fee")]
        public int? FeeId { get; set; }
        public virtual Sch_Fee Fee { get; set; }

        public string Recurrence { get; set; }

        public Decimal Amount { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
