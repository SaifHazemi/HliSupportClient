using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HliSupportClient.Models
{
    public class Incident
    {
        public string title { get; set; }
        public string ticketnumber { get; set; }
        public string AccountName { get; set; }
        public string ContactName { get; set; }
        public string prioritycode { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime createdon { get; set; }
        public Guid incidentid { get; set; }

    }
}