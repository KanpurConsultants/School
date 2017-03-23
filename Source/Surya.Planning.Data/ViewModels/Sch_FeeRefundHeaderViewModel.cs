using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Surya.India.Model.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Surya.India.Model.ViewModels
{
    public class Sch_FeeRefundHeaderViewModel 
    {
        [Display(Name = "Fee Refund Header Id")]
        public int FeeRefundHeaderId { get; set; }

        [Display(Name = "fee Refund Type"), Required]
        [ForeignKey("DocType")]
        [Index("IX_FeeRefundHeader_DocID", IsUnique = true, Order = 1)]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [Display(Name = "Fee Refund Date"), Required]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DocDate { get; set; }

        [Display(Name = "Fee Refund No"), Required, MaxLength(20)]
        [Index("IX_FeeRefundHeader_DocID", IsUnique = true, Order = 2)]
        public string DocNo { get; set; }

        [Display(Name = "Division"), Required]
        [ForeignKey("Division")]
        [Index("IX_FeeRefundHeader_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [ForeignKey("Site"), Display(Name = "Site")]
        [Index("IX_FeeRefundHeader_DocID", IsUnique = true, Order = 4)]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }


        public int StudentId { get; set; }
        public virtual string StudentName { get; set; }

        public string ProgramName { get; set; }

        public string StreamName { get; set; }

        public string ClassName { get; set; }

        public Decimal Amount { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDate { get; set; }

        public ICollection<Sch_FeeRefundLine> Sch_FeeRefundLines { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
