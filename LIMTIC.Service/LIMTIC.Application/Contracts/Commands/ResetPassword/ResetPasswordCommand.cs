using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMTIC.Application.Contracts.Commands.ResetPassword
{
    public class ResetPasswordCommand
    {
        public string email { get; set; }
        public string NewPassword { get; set; }
        public string ResetToken { get; set; }


    }
}
