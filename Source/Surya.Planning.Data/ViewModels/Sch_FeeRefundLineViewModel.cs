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
    public class Sch_FeeRefundLineViewModel
    {
        public int FeeRefundLineId { get; set; }

        [Display(Name = "Purchase Order")]
        [ForeignKey("FeeRefundHeader")]
        public int FeeRefundHeaderId { get; set; }
        public virtual Sch_FeeRefundHeader FeeRefundHeader { get; set; }

        [Display(Name = "Fee Receive Line")]
        [ForeignKey("FeeReceiveLine")]
        public int FeeReceiveLineId { get; set; }
        public virtual Sch_FeeReceiveLine FeeReceiveLine { get; set; }
        
        public int? FeeId { get; set; }
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

    public class Sch_FeeRefundMasterDetailModel
    {
        public int FeeRefundHeaderId { get; set; }
        public List<Sch_FeeRefundLineViewModel> FeeRefundLineViewModel { get; set; }
    }



}
