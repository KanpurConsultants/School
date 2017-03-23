using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Surya.India.Model.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Surya.India.Model.ViewModels
{
    public class Sch_PromotionHeaderViewModel 
    {
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }


        [ForeignKey("FromProgram"), Display(Name = "From Program")]
        public int FromProgramId { get; set; }
        public string FromProgramName { get; set; }


        [ForeignKey("FromStream"), Display(Name = "From Stream")]
        public int FromStreamId { get; set; }
        public string FromStreamName { get; set; }



        [ForeignKey("FromClass"), Display(Name = "From Class")]
        public int FromClassId { get; set; }
        public string FromClassName { get; set; }



        [ForeignKey("ToProgram"), Display(Name = "To Program")]
        public int ToProgramId { get; set; }
        public string ToProgramName { get; set; }


        [ForeignKey("ToStream"), Display(Name = "To Stream")]
        public int ToStreamId { get; set; }
        public string ToStreamName { get; set; }



        [ForeignKey("ToClass"), Display(Name = "To Class")]
        public int ToClassId { get; set; }
        public string ToClassName { get; set; }





    }
}
