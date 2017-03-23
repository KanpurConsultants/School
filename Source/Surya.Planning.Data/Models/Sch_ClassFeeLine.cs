using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_ClassFeeLine : EntityBase, IHistoryLog
    {
        public Sch_ClassFeeLine()
        {
        }

        [Key]
        public int ClassFeeLineId { get; set; }

        [ForeignKey("ClassFee"), Display(Name = "Class Fee")]
        public int ClassFeeId { get; set; }
        public virtual Sch_ClassFee ClassFee { get; set; }


        [ForeignKey("Fee"), Display(Name = "Fee")]
        public int FeeId { get; set; }
        public virtual Sch_Fee Fee { get; set; }

        [ForeignKey("LedgerAccount"), Display(Name = "LedgerAccount")]
        public int LedgerAccountId { get; set; }
        public virtual LedgerAccount LedgerAccount { get; set; }

        public string Recurrence { get; set; }

        public Decimal Amount { get; set; }

        [Display(Name = "Is Active ?")]
        public Boolean IsActive { get; set; }

        [Display(Name = "Is System Define ?")]
        public Boolean IsSystemDefine { get; set; }

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
