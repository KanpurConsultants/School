using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_FeeRefundLine : EntityBase, IHistoryLog
    {
        public Sch_FeeRefundLine()
        {

        }

        [Key]        
        public int FeeRefundLineId { get; set; }

        [Display(Name = "Purchase Order")]
        [ForeignKey("FeeRefundHeader")]
        public int FeeRefundHeaderId { get; set; }
        public virtual Sch_FeeRefundHeader  FeeRefundHeader { get; set; }

        [Display(Name = "Fee Receive Line")]
        [ForeignKey("FeeReceiveLine")]
        public int FeeReceiveLineId { get; set; }
        public virtual Sch_FeeReceiveLine FeeReceiveLine { get; set; }

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
