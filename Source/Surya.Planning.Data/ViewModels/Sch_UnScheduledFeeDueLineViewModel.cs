﻿using System.ComponentModel.DataAnnotations;

// New namespace imports:
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using Surya.India.Model.Models;
using System;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Surya.India.Model.ViewModels
{
    public class Sch_UnScheduledFeeDueLineViewModel
    {
        [Key]
        public int FeeDueLineId { get; set; }

        [Display(Name = "Purchase Order")]
        [ForeignKey("FeeDueHeader")]
        public int FeeDueHeaderId { get; set; }
        public virtual Sch_FeeDueHeader FeeDueHeader { get; set; }

        [ForeignKey("Admission"), Display(Name = "Admission")]
        public int AdmissionId { get; set; }
        public virtual Sch_Admission Admission { get; set; }

        public string StudentName { get; set; }


        [Display(Name = "Fee")]
        [ForeignKey("Fee")]
        public int? FeeId { get; set; }
        public virtual Sch_Fee Fee { get; set; }

        public string FeeName { get; set; }

        public string Recurrence { get; set; }

        [ForeignKey("Program"), Display(Name = "Program")]
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }

        [ForeignKey("Stream"), Display(Name = "Stream")]
        public int StreamId { get; set; }
        public string StreamName { get; set; }

        [ForeignKey("Class"), Display(Name = "Class")]
        public int ClassId { get; set; }
        public string ClassName { get; set; }

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


    public class Sch_UnScheduledFeeDueLineFilterViewModel
    {
        public int DocTypeId { get; set; }
        public int FeeDueHeaderId { get; set; }
        public string ProgramId { get; set; }
        public string ClassId { get; set; }
        public string StreamId { get; set; }
        public string ClassSectionId { get; set; }

        public int FeeId { get; set; }

        [Required]
        public Decimal Amount { get; set; }
    }
    public class Sch_UnScheduledFeeDueMasterDetailModel
    {
        public List<Sch_UnScheduledFeeDueLineViewModel> UnScheduledFeeDueLineViewModel { get; set; }
    }
}

