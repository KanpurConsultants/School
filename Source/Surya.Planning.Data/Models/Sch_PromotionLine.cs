using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_PromotionLine : EntityBase, IHistoryLog
    {
        public Sch_PromotionLine()
        {
            //PromotionLines = new List<PromotionLine>();
        }

        [Key]
        public int PromotionLineId { get; set; }
                        

        [ForeignKey("PromotionHeader"), Display(Name = "Promotion No")]
        public int PromotionHeaderId { get; set; }
        public virtual Sch_PromotionHeader PromotionHeader { get; set; }

        [ForeignKey("Student"), Display(Name = "Student")]
        public int StudentId { get; set; }
        public virtual Sch_Student Student { get; set; }


        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Status")]
        public int Status { get; set; }                

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
