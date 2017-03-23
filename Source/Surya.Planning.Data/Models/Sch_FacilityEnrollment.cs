using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_FacilityEnrollment : EntityBase, IHistoryLog
    {
        public Sch_FacilityEnrollment()
        {
        }

        [Key]
        public int FacilityEnrollmentId { get; set; }


        [Display(Name = "Facility Enrollment Type"), Required]
        [ForeignKey("DocType")]
        [Index("IX_FacilityEnrollment_DocID", IsUnique = true, Order = 1)]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        [Display(Name = "Facility Enrollment Date"), Required]
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DocDate { get; set; }

        [Display(Name = "Facility Enrollment No"), Required, MaxLength(20)]
        [Index("IX_FacilityEnrollment_DocID", IsUnique = true, Order = 2)]
        public string DocNo { get; set; }

        [Display(Name = "Division"), Required]
        [ForeignKey("Division")]
        [Index("IX_FacilityEnrollment_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual Division Division { get; set; }

        [ForeignKey("Site"), Display(Name = "Site")]
        [Index("IX_FacilityEnrollment_DocID", IsUnique = true, Order = 4)]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }
        
        [ForeignKey("Admission"), Display(Name = "Admission")]
        public int AdmissionId { get; set; }
        public virtual Sch_Admission Admission { get; set; }


        [ForeignKey("FacilitySubCategory"), Display(Name = "Facility Sub Category")]
        public int FacilitySubCategoryId { get; set; }
        public virtual Sch_FacilitySubCategory FacilitySubCategory { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }


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
