using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMTIC.WebAPI.Base;

namespace LIMTIC.WebAPI.Models.UserManagement.VerifyResetCode
{
    public class VerifyResetCodeResponse : BaseResponse
    {
        public string resetToken { get; set; }
    }
}
