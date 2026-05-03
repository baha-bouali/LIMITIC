using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMTIC.WebAPI.Base;

namespace LIMTIC.WebAPI.Models.UserManagement.ChangeUserPassword
{
    public class ChangeUserPasswordResponse : BaseResponse
    {
        public UserDto? User { get; set; }
    }
}
