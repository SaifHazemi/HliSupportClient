using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HliSupportClient.Models
{
   
    public class Contact
    {
       
        public string fullname { get; set; }
        public string  telephone1 { get; set; }
        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "ekteb haja")]
        public string  emailaddress1 { get; set; }
        [Display(Name = "User Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "ekteb haja")]
        public string password { get; set; }
        public Guid contactId { get; set; }

        public static string resetPasswordCode { get; set; }
        public static Guid ResetPasswordContactId { get; set; }


    }
  

}