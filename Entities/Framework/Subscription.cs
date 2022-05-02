namespace Entities.Framework
{
    public class Subscription : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal MonthPrice { get; set; }
        public decimal ThreeMonthPrice { get; set; }
        public decimal OneYearPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
