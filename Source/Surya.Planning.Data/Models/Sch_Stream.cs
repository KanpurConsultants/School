using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_Stream : EntityBase, IHistoryLog
    {
        public Sch_Stream()
        {
        }

        [Key]
        public int StreamId { get; set; }

        [ForeignKey("Program"), Display(Name = "Program")]
        public int ProgramId { get; set; }
        public virtual Sch_Program Program { get; set; }


        [Display (Name="Name")]
        [MaxLength(50), Required]
        [Index("IX_Stream_StreamName", IsUnique = true)]
        public string StreamName { get; set; }

        [Display(Name = "W.E.F.")]
        public DateTime WEF { get; set; }

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
