using Skender.Stock.Indicators;

namespace AxIndicators.Model
{
    public interface IAxIndicator
    {
        public string Title { get; }
        IEnumerable<IReusableResult> Calculate(object arg);
    }

    public class Close : IAxIndicator
    {
        public IEnumerable<IReusableResult> Calculate(object arg)
        {
            if (arg is List<Quote> list)
                return list.Select(x => new RsiResult(DateTime.Now) { Rsi = (double)x.Close });
            throw new Exception("arg is not Quote type");
        }

        public string Title => "Close";
    }

    public class Rsi : IAxIndicator
    {
        private readonly int _period;

        public Rsi(int period)
        {
            _period = period;
        }
        public IEnumerable<IReusableResult> Calculate(object arg)
        {
            if (arg is IEnumerable<IReusableResult> list)
                return list.GetRsi(_period);
            throw new Exception("arg is not Quote type");
        }

        public string Title => "RSI";

    }


}