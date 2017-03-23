using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_PromotionHeader : EntityBase, IHistoryLog
    {
        public Sch_PromotionHeader()
        {
            //PromotionLines = new List<PromotionLine>();
        }

        [Key]
        [Display(Name = "Promotion Id")]
        public int PromotionHeaderId { get; set; }
                        
        [Display(Name = "Order Type"),Required]
        [ForeignKey("DocType")]
        [Index("IX_PromotionHeader_DocID", IsUnique = true, Order = 1)]
        public int DocTypeId { get; set; }
        public virtual DocumentType DocType { get; set; }
                
        [Display(Name = "Order Date"),Required ]        
        [DisplayFormat(DataFormatString = "{0:dd/MMM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime  DocDate { get; set; }
        
        [Display(Name = "Order No"),Required,MaxLength(20) ]
        [Index("IX_PromotionHeader_DocID", IsUnique = true, Order = 2)]
        public string DocNo { get; set; }

        [Display(Name = "Division"),Required ]
        [ForeignKey("Division")]
        [Index("IX_PromotionHeader_DocID", IsUnique = true, Order = 3)]
        public int DivisionId { get; set; }
        public virtual  Division  Division {get; set;}

        [ForeignKey("Site"), Display(Name = "Site")]
        [Index("IX_PromotionHeader_DocID", IsUnique = true, Order = 4)]
        public int SiteId { get; set; }
        public virtual Site Site { get; set; }

        [ForeignKey("FromProgram"), Display(Name = "From Program")]
        public int FromProgramId { get; set; }
        public virtual Sch_Program FromProgram { get; set; }

        [ForeignKey("FromStream"), Display(Name = "From Stream")]
        public int FromStreamId { get; set; }
        public virtual Sch_Stream FromStream { get; set; }

        [ForeignKey("FromClass"), Display(Name = "From Class")]
        public int FromClassId { get; set; }
        public virtual Sch_Class FromClass { get; set; }



        [ForeignKey("ToProgram"), Display(Name = "To Program")]
        public int ToProgramId { get; set; }
        public virtual Sch_Program ToProgram { get; set; }

        [ForeignKey("ToStream"), Display(Name = "To Stream")]
        public int ToStreamId { get; set; }
        public virtual Sch_Stream ToStream { get; set; }

        [ForeignKey("ToClass"), Display(Name = "To Class")]
        public int ToClassId { get; set; }
        public virtual Sch_Class ToClass { get; set; }






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

        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDate { get; set; }

        //public ICollection<Sch_PromotionLine> Sch_PromotionLines { get; set; }

        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
