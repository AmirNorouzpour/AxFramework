using System;
using System.Collections.Generic;
using System.Linq;

namespace API
{
    public class AxRsi
    {
        public List<AxOhlc> Ohlcs { get; set; }
        public AxRsi(int period)
        {
            Args = new RsiArgs { Period = period };
        }

        public void LoadArgs(int period)
        {
            Args = new RsiArgs
            {
                Period = period,
            };
        }
        public List<BsSignal> BsSignals = new();
        public RsiArgs Args { get; set; }

        public List<AxOhlc> Calculate()
        {
            BsSignals.Clear();
            for (var i = 0; i < Ohlcs.Count; i++)
            {
                var ohlc = Ohlcs[i];
                ohlc.Index = i;
                if (i == 0)
                    continue;
                var perOhlc = Ohlcs[i - 1];

                var temp = new BsSignal();

                ohlc.Change = (decimal)ohlc.Close - (decimal)perOhlc.Close;
                if (ohlc.Change > 0)
                    ohlc.Gain = ohlc.Change;
                else if (ohlc.Change < 0)
                    ohlc.Loss = -1 * ohlc.Change;
                if (i >= Args.Period)
                {
                    var min = i - Args.Period + 1;
                    var max = Args.Period;
                    if (i == Args.Period)
                    {
                        var models = Ohlcs.Where(x => x.Index >= min && x.Index <= max);
                        ohlc.AvgGain = SumG(models) / Args.Period;
                        ohlc.AvgLoss = SumL(models) / Args.Period;
                    }
                    else
                    {
                        ohlc.AvgGain = (perOhlc.AvgGain * (Args.Period - 1) + ohlc.Gain) / Args.Period;
                        ohlc.AvgLoss = (perOhlc.AvgLoss * (Args.Period - 1) + ohlc.Loss) / Args.Period;
                    }
                    if (ohlc.AvgLoss == 0)
                        continue;
                    ohlc.Rs = ohlc.AvgGain / ohlc.AvgLoss;
                    ohlc.Rsi = 100 - 100 / (1 + ohlc.Rs);
                    temp.Rsi = ohlc.Rsi;
                    temp.Side = temp.Rsi > 75 ? SignalSide.Short : temp.Rsi < 25 ? SignalSide.Short : SignalSide.None;
                    temp.HaClose = ohlc.Close;
                    temp.OpenTime = ohlc.Date;
                    //if (BsSignals.Count < 3)
                    //    continue;

                    //var lastSignal = BsSignals[BsSignals.Count - 2];

                    if (i < Args.Period * 2)
                        continue;
                    BsSignals.Add(temp);
                }
            }
            var resCount = BsSignals.Count - 1;
            var rsi = Ohlcs.LastOrDefault()!.Rsi.ToString();
            return Ohlcs;
        }

        private decimal SumG(IEnumerable<AxOhlc> models)
        {
            var sum = (decimal)0;
            foreach (var model in models)
            {
                sum += model.Gain;
            }
            return sum;
        }

        private decimal SumL(IEnumerable<AxOhlc> models)
        {
            var sum = (decimal)0;
            foreach (var model in models)
            {
                sum += model.Loss;
            }
            return sum;
        }

        public double Rsi { get; set; }

        public void Load(List<AxOhlc> ohlc)
        {
            Ohlcs = ohlc;
        }
    }

    public class RsiArgs : IndicatorArgs
    {
        public int Period { get; set; }
    }

    public class RSISerie
    {
        public List<double?> RSI { get; set; }
        public List<double?> RS { get; set; }

        public RSISerie()
        {
            RSI = new List<double?>();
            RS = new List<double?>();
        }
    }

    public class IndicatorArgs
    {

    }

    public class AxOhlc
    {
        public DateTime Date { get; set; }

        public decimal Open { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public decimal Close { get; set; }

        public decimal Volume { get; set; }

        public decimal AdjClose { get; set; }
        public int Index { get; set; }
        public decimal Change { get; set; }
        public decimal Gain { get; set; }
        public decimal Loss { get; set; }
        public decimal AvgGain { get; set; }
        public decimal AvgLoss { get; set; }
        public decimal Rs { get; set; }
        public decimal Rsi { get; set; }
        public decimal Hlc3 => (High + Low + Close) / 3;
        public long Id { get; set; }
    }
    public class BsSignal
    {
        public decimal Rsi { get; set; }
        public decimal? Cci { get; set; }
        public decimal HaClose { get; set; }
        public decimal HaOpen { get; set; }
        public decimal HaHigh { get; set; }
        public decimal HaLow { get; set; }
        public SignalSide Side { get; internal set; }
        public double Rsix { get; internal set; }
        public SignalSide RealSide { get; internal set; }
        public DateTime OpenTime { get; set; }
        public decimal Ma { get; set; }
        public List<decimal> Levels { get; set; }
        public bool SqzOff { get; set; }
        public decimal SlowSma { get; set; }
        public SignalSide SignalSide { get; set; }
        public bool SignalUsed { get; set; }
        public decimal OpenPrice { get; set; }
        //public SignalStatus Status { get; set; }
        public decimal MdemEma { get; set; }
        public decimal FastEma { get; set; }
        public bool SqzOn { get; set; }
        public decimal Diff { get; set; }
        public bool IsBearish { get; set; }
        public bool IsBullish { get; set; }
        public int SideValue { get; set; }
        public decimal RsiHigh { get; set; }
        public decimal RsiLow { get; set; }
        public double? Atr { get; set; }
        public decimal Up { get; set; }
        public decimal Down { get; set; }
        public decimal Tickrng { get; set; }
        public decimal Pvi { get; set; }
        public decimal Nvi { get; set; }
        public decimal Pdiv { get; set; }
        public decimal Ndiv { get; set; }

        public override string ToString()
        {
            return Side + $" ({Ma:n2}) at " + OpenTime;
        }
    }

    public enum SignalSide
    {
        None = -1,
        Long = 0,
        Short = 1
    }

}
