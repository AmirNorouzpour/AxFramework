namespace AxIndicators.Model
{
    public class OHLC
    {
        public long Id { get; set; }
        public int Index { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Change { get; set; }
        public decimal Gain { get; set; }
        public decimal Loss { get; set; }
        public decimal AvgGain { get; set; }
        public decimal AvgLoss { get; set; }
        public decimal Rs { get; set; }
        public decimal Rsi { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }
        public double Volume { get; set; }
        public long OpenTimeTicks { get; set; }
        public decimal AdjClose { get; set; }
    }
}
