using Surya.India.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Surya.India.Model.DatabaseViews
{

    [Table("ViewStudentCurrentAdmission")]
    public class ViewStudentCurrentAdmission
    {
        [Key]
        public int PersonID { get; set; }
        public int? AdmissionId { get; set; }
    }
}
