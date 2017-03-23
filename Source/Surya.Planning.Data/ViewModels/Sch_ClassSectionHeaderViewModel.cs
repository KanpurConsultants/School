using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Surya.India.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Surya.India.Model.ViewModel
{
    public class Sch_ClassSectionHeaderViewModel
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }

        public int StreamId { get; set; }
        public string StreamName { get; set; }

        public int ClassId { get; set; }
        public string ClassName { get; set; }
    }
}
