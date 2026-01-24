using PCM.Api.Models.Members;

namespace PCM.Api.Models
{
    public class WalletTransaction
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public decimal Amount { get; set; }   // âm = trừ tiền
        public string Type { get; set; } = null!;       // ENTRY_FEE
        public string Description { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
