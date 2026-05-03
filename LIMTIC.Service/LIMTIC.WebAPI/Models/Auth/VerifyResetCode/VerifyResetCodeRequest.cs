using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMTIC.WebAPI.Models.Auth.VerifyResetCode
{
    public class VerifyResetCodeRequest
    {
        public string email { get; set; }
        public string otpToken { get; set; }
    }
}
