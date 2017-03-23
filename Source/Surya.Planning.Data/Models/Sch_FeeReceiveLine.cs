using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_FeeReceiveLine : EntityBase, IHistoryLog
    {
        public Sch_FeeReceiveLine()
        {

        }

        [Key]        
        public int FeeReceiveLineId { get; set; }

        [Display(Name = "Purchase Order")]
        [ForeignKey("FeeReceiveHeader")]
        public int FeeReceiveHeaderId { get; set; }
        public virtual Sch_FeeReceiveHeader  FeeReceiveHeader { get; set; }

        [Display(Name = "Fee Due Line")]
        [ForeignKey("FeeDueLine")]
        public int FeeDueLineId { get; set; }
        public virtual Sch_FeeDueLine FeeDueLine { get; set; }

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
