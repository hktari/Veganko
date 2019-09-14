using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.Account
{
    public class ValidateOTPInput
    {
        [Required]
        public int OTP { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
