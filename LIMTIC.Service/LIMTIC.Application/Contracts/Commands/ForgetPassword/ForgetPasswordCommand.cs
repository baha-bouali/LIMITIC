using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMTIC.Application.Contracts.Commands.ForgetPassword
{
    public class ForgetPasswordCommand
    {
        public string Email { get; set; }
    }
}
