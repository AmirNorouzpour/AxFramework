using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Framework
{
    public class StrategyIndicator : BaseEntity
    {
        [ForeignKey("StrategyId")]
        public int StrategyId { get; set; }

        public Strategy Strategy { get; set; }
        public string IndicatorTypeName { get; set; }
        public string ConfigData { get; set; }
    }
}
