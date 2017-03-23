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
    public class Sch_FeeDueHeaderViewModel 
    {
        [Key]
        [Display(Name = "Fee Due Id")]
        public int FeeDueHeaderId { get; set; }

        [Display(Name = "Fee Due Type"), Required]
        [ForeignKey("DocType")]
        [Index("IX_FeeDueHeader_DocID", IsUnique = true, Order = 1)]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [Display(Name = "Fee Due Date"), Required]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DocDate { get; set; }

        [Display(Name = "Fee Due No"), Required, MaxLength(20)]
        [Index("IX_FeeDueHeader_DocID", IsUnique = true, Order = 2)]
        public string DocNo { get; set; }

        [Display(Name = "Division"), Required]
        [ForeignKey("Division")]
        [Index("IX_FeeDueHeader_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [ForeignKey("Site"), Display(Name = "Site")]
        [Index("IX_FeeDueHeader_DocID", IsUnique = true, Order = 4)]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }

        public DateTime FromDate { get; set; }

        [ForeignKey("Program"), Display(Name = "Program")]
        public int? ProgramId { get; set; }
        public virtual Sch_Program Program { get; set; }

        [ForeignKey("Stream"), Display(Name = "Stream")]
        public int? StreamId { get; set; }
        public virtual Sch_Stream Stream { get; set; }

        [ForeignKey("Class"), Display(Name = "Class")]
        public int? ClassId { get; set; }
        public virtual Sch_Class Class { get; set; }


        public DateTime ToDate { get; set; }

        [Display(Name = "Last Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime LastDate { get; set; }

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

        public ICollection<Sch_FeeDueLine> Sch_FeeDueLines { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }

    }
}
