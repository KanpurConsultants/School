﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_Facility : EntityBase, IHistoryLog
    {
        public Sch_Facility()
        {
        }

        [Key]
        public int FacilityId { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50), Required]
        public string FacilityName { get; set; }

        [Display(Name = "Sub Category Title")]
        [MaxLength(50), Required]
        public string FacilitySubCategoryTitle { get; set; }

        
        [ForeignKey("LedgerAccount"), Display(Name = "LedgerAccount")]
        public int LedgerAccountId { get; set; }
        public virtual LedgerAccount LedgerAccount { get; set; }

        public string Recurrence { get; set; }

        public bool RenewOnPromotion { get; set; }




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
