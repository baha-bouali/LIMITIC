using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMTIC.Domain.Entities
{
    public class ResetPasswordEntity
    {   
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string OTPTokenHash { get; set; } = string.Empty;
        public DateTime OTPTokenExpiry { get; set; }
        public string? ResetPasswordTokenHash { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
        public User? User { get; set; }
    }
}
