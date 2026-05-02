using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMTIC.WebAPI.Base;

namespace LIMTIC.WebAPI.Models.UserManagement.ChangeUserPassword
{
    internal class ChangeUserPasswordResponse : BaseResponse
    {
        public UserDto? User { get; set; }
    }
}
