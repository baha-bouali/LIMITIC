using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMTIC.Application.Contracts.Commands.VerifyResetCode
{
    public class VerifyResetCodeCommandResponse
    {
       public string ResetToken { get; set; }
    }
}
