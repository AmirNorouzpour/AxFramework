using System;
using System.Linq.Expressions;
using Common;
using Entities.Framework;

namespace API.Models
{
    public class DataRequest
    {
        public string Sort { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public SortType SortType { get; set; }
    }
}
