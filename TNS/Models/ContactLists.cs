using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TNS.Models
{
    public class ContactLists
    {
        public string Seeds { get; set; }
        public string ID { get; set; }
        public string Listname { get; set; }
        public string Datecreated { get; set; }
        public string CreatedBy { get; set; }
    }
}