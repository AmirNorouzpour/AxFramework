namespace Entities.Framework
{
    public class Symbol : BaseEntity
    {
        public string Name { get; set; }
        public string Ticker { get; set; }
        public int Decimals { get; set; }
        public int LotSize { get; set; }
        public bool IsActive { get; set; }
        public string Logo { get; set; }
    }
}
