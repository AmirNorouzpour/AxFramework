using Microsoft.EntityFrameworkCore;

namespace Entities.Framework
{
    public class Subscription : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        [Precision(18, 8)]
        public decimal MonthPrice { get; set; }
        [Precision(18, 8)]
        public decimal ThreeMonthPrice { get; set; }
        [Precision(18, 8)]
        public decimal OneYearPrice { get; set; }
        [Precision(18, 8)]
        public decimal Discount { get; set; }
    }
}
