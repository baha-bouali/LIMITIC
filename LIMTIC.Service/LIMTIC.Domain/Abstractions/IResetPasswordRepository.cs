using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMTIC.Domain.Entities;

namespace LIMTIC.Domain.Abstractions
{
    public interface IResetPasswordRepository
    {
        Task AddOTPTokenAsync(ResetPassword resetPassword);
        Task <ResetPassword?> GetTokenAsync(Guid userId);
        Task <ResetPassword> UpdateResetPasswordTokenAsync(ResetPassword resetPassword);
        Task RevokeTokenAsync(Guid userId);
    }
}
