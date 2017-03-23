using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Login.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        public string Name { get; set; }

        //[ForeignKey("Manager")]
        //public int? ManagerId { get; set; }
        //public virtual Employee Manager { get; set; }
    }
}