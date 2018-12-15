using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TNS.Models
{

    public class Campaign
    {
        public string ID { get; set; }
        public string CampaignName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string EmailTitle { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string SenderName { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string SenderEmail { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string StartDate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string EndDate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string ListID { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string CC { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string Bcc { get; set; }

        [AllowHtml]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string ParamiterizedTemplate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(AllowEmptyStrings = true)]
        public string Recipient { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Attachment { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DateCreated { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CreatedBy { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Cycle { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string SMTPResp { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string deliveryType { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string campaignType { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Status { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string LastRunTime { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string NextRunTime { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DeactivatedBy { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Datedeactivated { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string schedule { get; set; }

    }
}