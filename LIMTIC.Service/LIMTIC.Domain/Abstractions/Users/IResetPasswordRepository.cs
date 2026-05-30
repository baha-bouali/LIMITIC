using LIMTIC.Domain.Entities.ResetPassword;

namespace LIMTIC.Domain.Abstractions.Users
{
    public interface IResetPasswordRepository
    {
        Task AddResetPasswordAsync(ResetPasswordEntity resetPassword);
        Task<ResetPasswordEntity?> GetResetPasswordAsync(Guid userId);
        Task<ResetPasswordEntity> UpdateResetPasswordAsync(ResetPasswordEntity resetPassword);
        Task DeleteResetPasswordAsync(Guid userId);
    }
}
