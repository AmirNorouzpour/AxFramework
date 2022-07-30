using AxIndicators.Model;

namespace AxIndicators
{
    public interface IAxIndicator<T>
    {
        protected List<OHLC> OhlcList { get; set; }
        public T Calculate();

    }


}