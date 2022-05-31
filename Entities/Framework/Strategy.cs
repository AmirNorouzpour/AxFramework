using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Entities.Framework
{
    public class Strategy : BaseEntity
    {
        public string Name { get; set; }
        [Precision(18, 8)]
        public decimal OrderSize { get; set; }
        [Precision(18, 8)]
        public decimal InitialCapital { get; set; }
        public string BaseCurrency { get; set; }
        public int Pyramiding { get; set; }
        public OrderSizeType OrderSizeType { get; set; }
        [Precision(18, 8)]
        public decimal Commission { get; set; }
        public List<StrategyIndicator> IndicatorList { get; set; }
    }

    public enum OrderSizeType
    {
        Contract,
        Composite,
        MonetaryUnit

    }
}
