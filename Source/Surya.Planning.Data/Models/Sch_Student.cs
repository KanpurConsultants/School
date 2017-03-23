using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.Models
{
    public class Sch_Student : EntityBase
    {       

        public Sch_Student()
        {            
        }

        [Key]
        [ForeignKey("Person"), Display(Name = "Person")]
        public int PersonID { get; set; }
        public virtual Person Person { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }
        public DateTime DOB { get; set; }

        [MaxLength(20)]
        public string CastCategory { get; set; }

        [MaxLength(50)]
        public string Religion { get; set; }

        [MaxLength(100)]
        public string FatherName { get; set; }

        [MaxLength(100)]
        public string MotherName { get; set; }




        [MaxLength(50)]
        public string OMSId { get; set; }
    }
}
