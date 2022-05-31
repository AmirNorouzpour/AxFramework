using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Framework
{
    public class Payment : BaseEntity
    {
        [ForeignKey("SubscriptionId")]
        public int SubscriptionId { get; set; }

        public Subscription Subscription { get; set; }
        [Precision(18, 8)]
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string TransactionId { get; set; }
        public string Coin { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
        public int DurationDays { get; set; }
    }

    public enum PaymentStatus
    {
    }
}
