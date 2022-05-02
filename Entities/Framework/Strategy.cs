using System.Collections.Generic;

namespace Entities.Framework
{
    public class Strategy : BaseEntity
    {
        public string Name { get; set; }
        public decimal OrderSize { get; set; }
        public decimal InitialCapital { get; set; }
        public decimal BaseCurrency { get; set; }
        public int Pyramiding { get; set; }
        public OrderSizeType OrderSizeType { get; set; }
        public decimal Commission { get; set; }
        public List<Indicator> IndicatorList { get; set; }
    }

    public enum OrderSizeType
    {
        Contract,
        Composite,
        MonetaryUnit

    }
}
