using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_Class : EntityBase, IHistoryLog
    {
        public Sch_Class()
        {
        }

        [Key]
        public int ClassId { get; set; }

        [ForeignKey("Program"), Display(Name = "Program")]
        public int ProgramId { get; set; }
        public virtual Sch_Program Program { get; set; }


        [Display (Name="Name")]
        [MaxLength(50), Required]
        [Index("IX_Class_ClassName", IsUnique = true)]
        public string ClassName { get; set; }

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
