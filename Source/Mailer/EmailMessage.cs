using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Surya.India.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Mailer.Model
{
    public class EmailMessage
    {
        public virtual string Body { get; set; }
        public virtual string To { get; set; }
        public virtual string Subject { get; set; }
        public virtual string CC { get; set; }

    }
}
