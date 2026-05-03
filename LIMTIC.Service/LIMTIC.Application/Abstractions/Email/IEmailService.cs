using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMTIC.Application.Abstractions.Email
{
    public interface IEmailService
    {
        Task SendOTPEmailAsync(string toEmail, string otp);
    }
}
