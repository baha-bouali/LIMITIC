using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMTIC.WebAPI.Base;

namespace LIMTIC.WebAPI.Models.Auth.VerifyResetCode
{
    public class VerifyResetCodeResponse : BaseResponse
    {
        public string ResetToken { get; set; }
    }
}
