﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_FacilitySubCategory : EntityBase, IHistoryLog
    {
        public Sch_FacilitySubCategory()
        {
        }

        [Key]
        public int FacilitySubCategoryId { get; set; }

        [ForeignKey("Facility"), Display(Name = "Facility  No")]
        public int FacilityId { get; set; }
        public virtual Sch_Facility Facility { get; set; }


        [Display(Name = "Name")]
        [MaxLength(50), Required]
        public string FacilitySubCategoryName { get; set; }

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
