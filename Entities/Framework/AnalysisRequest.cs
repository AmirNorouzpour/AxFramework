using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Entities.Framework
{
    public class AnalysisRequest : BaseEntity
    {

        [ForeignKey("StrategyId")]
        public int? StrategyId { get; set; }
        public Strategy Strategy { get; set; }
        public List<Symbol> SymbolList { get; set; }
        public string TimeFrames { get; set; }
        [NotMapped]
        public List<TimeFrame> TimeFrameList => TimeFrames.Split(",").Select(x => (TimeFrame)Enum.Parse(typeof(TimeFrame), x, true)).ToList();
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }

    public enum TimeFrame
    {
    }
}
