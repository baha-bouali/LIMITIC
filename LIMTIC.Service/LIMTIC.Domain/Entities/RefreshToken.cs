namespace LIMTIC.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }

        public Guid UserId { get; set; }

        public User? User { get; set; }
    }
}
