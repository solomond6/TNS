using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TNS.Models
{
    public class Templates
    {
        [AllowHtml]
        public string Contents { get; set; }
        public string ID { get; set; }
        public string Title { get; set; }
        public string Datecreated { get; set; }
        public string CreatedBy { get; set; }
    }
}