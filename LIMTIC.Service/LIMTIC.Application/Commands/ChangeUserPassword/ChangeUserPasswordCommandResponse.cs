using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMTIC.Domain.Entities;

namespace LIMTIC.Application.Commands.ChangeUserPassword
{
    public class ChangeUserPasswordCommandResponse
    {
        public User User { get; set; }
    }
}
