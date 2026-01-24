using PCM.Api.Models.Members;

namespace PCM.Api.Models
{
    public class Wallet
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        public decimal Balance { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}