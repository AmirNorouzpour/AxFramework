using System.Collections.Generic;

namespace API.Models
{
    public class Indicator
    {
        public string title { get; set; }
        public string description { get; set; }
        public List<Parameter> parameters { get; set; }
    }

    public class Inout
    {
        public string id { get; set; }
        public string start { get; set; }
        public bool isComplete { get; set; }
        public string end { get; set; }
    }

    public class Parameter
    {
        public string title { get; set; }
        public string type { get; set; }
        public bool isInput { get; set; }
        public bool dataEntry { get; set; }
        public List<Inout> inouts { get; set; }
    }

    public class Model
    {
        public string name { get; set; }
        public List<Box> boxs { get; set; }
    }

    public class Box
    {
        public string title { get; set; }
        public string id { get; set; }
        public Indicator indicator { get; set; }
        public string transform { get; set; }
    }


}
