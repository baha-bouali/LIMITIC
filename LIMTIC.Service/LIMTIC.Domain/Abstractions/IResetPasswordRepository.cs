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
        Task AddResetPasswordAsync(ResetPasswordEntity resetPassword);
        Task <ResetPasswordEntity?> GetResetPasswordAsync(Guid userId);
        Task <ResetPasswordEntity> UpdateResetPasswordAsync(ResetPasswordEntity resetPassword);
        Task DeleteResetPasswordAsync(Guid userId);
    }
}
