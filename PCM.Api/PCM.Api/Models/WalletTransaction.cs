using PCM.Api.Enums;
using PCM.Api.Models.Members;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCM.Api.Models
{
    [Table("025_WalletTransactions")]
    public class WalletTransaction
    {
        [Key]
        public int Id { get; set; }

        // Giao dịch quỹ CLB (không cần MemberId)
        public int? MemberId { get; set; }
        public Member? Member { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType Type { get; set; }  // Income / Expense

        public string? Category { get; set; }      // Phí nhập CLB, Tiền sân, Giải thưởng, ...
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }     // UserId của người tạo
    }
}
