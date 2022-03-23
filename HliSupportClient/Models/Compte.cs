using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HliSupportClient.Models
{
    public class Compte
    {
        public string comptename { get; set; }
        public Guid accountid { get; set; }
        public string name { get; set; }
        public string emailaddress1 { get; set; }
        public string password { get; set; }
    }
}