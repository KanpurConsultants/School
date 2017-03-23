using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_Admission : EntityBase, IHistoryLog
    {
        public Sch_Admission()
        {
            //FeeDueLines = new List<FeeDueLine>();
        }

        [Key]
        [Display(Name = "Admission Id")]
        public int AdmissionId { get; set; }

        [Display(Name = "Admission Type"), Required]
        [ForeignKey("DocType")]
        [Index("IX_Admission_DocID", IsUnique = true, Order = 1)]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [Display(Name = "Admission Date"), Required]        
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime  DocDate { get; set; }

        [Display(Name = "Admission No"), Required, MaxLength(20)]
        [Index("IX_Admission_DocID", IsUnique = true, Order = 2)]
        public string DocNo { get; set; }

        [Display(Name = "Division"),Required ]
        [ForeignKey("Division")]
        [Index("IX_Admission_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual  Division  Division {get; set;}

        [ForeignKey("Site"), Display(Name = "Site")]
        [Index("IX_Admission_DocID", IsUnique = true, Order = 4)]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }


        [ForeignKey("AdmissionQuota"), Display(Name = "Admission Quota")]
        public int AdmissionQuotaId { get; set; }
        public virtual Sch_AdmissionQuota AdmissionQuota { get; set; }

        [ForeignKey("ClassSection"), Display(Name = "Section")]
        public int ClassSectionId { get; set; }
        public virtual Sch_ClassSection ClassSection { get; set; }

        [MaxLength(20)]
        public string RollNo { get; set; }


        [ForeignKey("Student"), Display(Name = "Admission Quota")]
        public int StudentId { get; set; }
        public virtual Sch_Student Student { get; set; }

        public string PromotionNo { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }

        [Display(Name = "Is Active ?")]
        public Boolean IsActive { get; set; }

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
