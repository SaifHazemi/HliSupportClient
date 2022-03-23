using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HliSupportClient.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage ="new Password required", AllowEmptyStrings =false)]
        public string NewPassword { get; set; }
        [Required]
        public string  ResetCode { get; set; }
    }
   
}