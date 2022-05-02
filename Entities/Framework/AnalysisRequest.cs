using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Framework
{
    public class AnalysisRequest:BaseEntity
    {

        [ForeignKey("StrategyId")]
        public int? StrategyId { get; set; }
        public Strategy Strategy { get; set; }
        public List<Symbol> SymbolList { get; set; }
        public List<TimeFrame> TimeFrameList { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }

    public enum TimeFrame
    {
    }
}
