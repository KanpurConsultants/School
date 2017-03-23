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
    public class Sch_ClassFeeViewModel
    {
        public int ClassFeeId { get; set; }

        public int ProgramId { get; set; }
        public string ProgramName { get; set; }

        public int StreamId { get; set; }
        public string StreamName { get; set; }

        public int ClassId { get; set; }
        public string ClassName { get; set; }

        public int AdmissionQuotaId { get; set; }
        public string AdmissionQuotaName { get; set; }


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
    }




    public class Sch_ClassFeeLineViewModel
    {
        public int ClassFeeLineId { get; set; }

        public int FeeId { get; set; }
        public string FeeName { get; set; }

        public int LedgerAccountId { get; set; }
        public string LedgerAccountName { get; set; }

        public string Recurrence { get; set; }

        public Decimal Amount { get; set; }

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
