using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Surya.India.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Surya.India.Model.ViewModel
{
    public class Sch_FacilityEnrollmentViewModel
    {
        public int FacilityEnrollmentId { get; set; }


        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }

        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public int DivisionId { get; set; }

        public int SiteId { get; set; }


        public int AdmissionId { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; }


        public int ProgramId { get; set; }
        public string ProgramName { get; set; }

        public int StreamId { get; set; }
        public string StreamName { get; set; }

        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public int FacilityId { get; set; }
        public string FacilityName { get; set; }

        public int FacilitySubCategoryId { get; set; }
        public string FacilitySubCategoryName { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public int AvailDays { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }


    }
}
