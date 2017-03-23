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
    public class Sch_FeeReceiveLineViewModel
    {
        public int FeeReceiveLineId { get; set; }

        [Display(Name = "Purchase Order")]
        [ForeignKey("FeeReceiveHeader")]
        public int FeeReceiveHeaderId { get; set; }
        public virtual Sch_FeeReceiveHeader FeeReceiveHeader { get; set; }

        [Display(Name = "Fee Due Line")]
        [ForeignKey("FeeDueLine")]
        public int FeeDueLineId { get; set; }
        public string FeeDueDocNo { get; set; }

        public string FeeName { get; set; }

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

    public class Sch_FeeReceiveMasterDetailModel
    {
        public int FeeReceiveHeaderId { get; set; }
        public List<Sch_FeeReceiveLineViewModel> FeeReceiveLineViewModel { get; set; }
    }



}
