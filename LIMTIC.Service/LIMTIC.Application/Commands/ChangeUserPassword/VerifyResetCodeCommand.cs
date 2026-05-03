using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMTIC.Application.Commands.ChangeUserPassword
{
   public class VerifyResetCodeCommand
    {
        public string email { get; set; }
        public string otpToken { get; set; }
    }
}
