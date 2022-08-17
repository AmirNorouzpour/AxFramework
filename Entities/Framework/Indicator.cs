using System.Collections.Generic;

namespace Entities.Framework
{
    public class IndicatorGroup
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public bool IsOpen { get; set; }
        public string Description { get; set; }
        public List<Indicator> Indicators { get; set; }
    }

    public class Indicator
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public List<IndicatorParameter> Parameters { get; set; }
    }

    public class IndicatorParameter
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public bool? IsInput { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
        public bool DataEntry { get; set; }
        public string Id { get; set; }
    }
}
