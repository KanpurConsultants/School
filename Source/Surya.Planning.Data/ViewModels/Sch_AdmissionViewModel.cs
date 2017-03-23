using System.ComponentModel.DataAnnotations;

// New namespace imports:
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Surya.India.Model.Models;
using System;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Surya.India.Model.ViewModels
{
    public class Sch_AdmissionViewModel
    {
        public Sch_AdmissionViewModel()
        {
            //FeeDueLines = new List<FeeDueLine>();
        }

        [Key]
        [Display(Name = "Admission Id")]
        public int AdmissionId { get; set; }

        public int PersonId { get; set; }


        public int DocTypeId { get; set; }
       
        public DateTime  DocDate { get; set; }

        public string DocNo { get; set; }

        public int DivisionId { get; set; }

        public int SiteId { get; set; }

        public int AdmissionQuotaId { get; set; }
        public string AdmissionQuotaName { get; set; }
        public int ClassSectionId { get; set; }
        public string ClassSectionName { get; set; }


        public int? ProgramId { get; set; }
        public string ProgramName { get; set; }

        public int? ClassId { get; set; }
        public string ClassName { get; set; }

        public int? StreamId { get; set; }
        public string StreamName { get; set; }



        public string RollNo { get; set; }
        public int ExistingStudentId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Suffix { get; set; }
        public string Code { get; set; }
        public string Gender { get; set; }
        public DateTime DOB { get; set; }
        public string CastCategory { get; set; }
        public string Religion { get; set; }


        public string Address { get; set; }

        public int? CityId { get; set; }
        public string CityName { get; set; }
        public string Zipcode { get; set; }

        public string Mobile { get; set; }

        public string EMail { get; set; }

        public string FatherName { get; set; }

        public string MotherName { get; set; }

        public string GuardianName { get; set; }

        public string GuardianMobile { get; set; }

        public string GuardianEMail { get; set; }


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
