﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_ClassFee : EntityBase, IHistoryLog
    {
        public Sch_ClassFee()
        {
        }

        [Key]
        public int ClassFeeId { get; set; }

        [ForeignKey("Program"), Display(Name = "Program")]
        public int ProgramId { get; set; }
        public virtual Sch_Program Program { get; set; }

        [ForeignKey("Stream"), Display(Name = "Stream")]
        public int StreamId { get; set; }
        public virtual Sch_Stream Stream { get; set; }

        [ForeignKey("Class"), Display(Name = "Class")]
        public int ClassId { get; set; }
        public virtual Sch_Class Class { get; set; }

        [ForeignKey("AdmissionQuota"), Display(Name = "Admission Quota")]
        public int AdmissionQuotaId { get; set; }
        public virtual Sch_AdmissionQuota AdmissionQuota { get; set; }

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
