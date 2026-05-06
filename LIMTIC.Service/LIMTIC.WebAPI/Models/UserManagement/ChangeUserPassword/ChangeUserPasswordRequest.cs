using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMTIC.WebAPI.Models.UserManagement.ChangeUserPassword
{
    public class ChangeUserPasswordRequest
    {
         public string Email { get; set; } = string.Empty;
         public string OldPassword { get; set; }
         public string NewPassword { get; set; }

    }
}
