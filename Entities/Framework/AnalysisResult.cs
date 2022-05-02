using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Framework
{
    public class AnalysisResult : BaseEntity
    {
        public decimal NetProfit { get; set; }
        public decimal NetProfitPercentage { get; set; }
        public decimal WinRate { get; set; }
        public int TotalTrades { get; set; }
        [ForeignKey("AnalysisRequestId")]
        public int AnalysisRequestId { get; set; }
        public AnalysisRequest AnalysisRequest { get; set; }
        [ForeignKey("SymbolId")]
        public int SymbolId { get; set; }
        public Symbol Symbol { get; set; } 
        public TimeFrame TimeFrame { get; set; }

    }
}
