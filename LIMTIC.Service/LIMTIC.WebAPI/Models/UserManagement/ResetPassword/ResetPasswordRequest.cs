using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMTIC.WebAPI.Base;

namespace LIMTIC.WebAPI.Models.UserManagement.ResetPassword
{
    public class ResetPasswordRequest
    {
        public string email { get; set; }
        public string NewPassword { get; set; }
        public string ResetToken { get; set; }
    
}
}
