using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities.Framework
{
    public class AnalysisResult : BaseEntity
    {
        [Precision(18, 8)]
        public decimal NetProfit { get; set; }
        [Precision(18, 2)]
        public decimal NetProfitPercentage { get; set; }
        [Precision(18, 2)]
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
