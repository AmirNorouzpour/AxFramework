using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Composition;
using System.Linq.Expressions;
using Common;
using Entities.Framework;
using NLog.Filters;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Models
{
    public class DataRequest
    {
        //string filter, string sort, int page, int PageSize, SortType SortType
        public List<AxFilter> Filters { get; set; }
        public string Sort { get; set; }
        public int PageSize { get; set; } = 50;
        public SortType SortType { get; set; }
        public int PageIndex { get; set; }
    }

    public class AxFilter : BaseEntity
    {
        public string Property { get; set; }
        public OperationType Operation { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public string Connector { get; set; }
        public bool IsCalculation { get; set; }

        //public string Type { get; set; }
    }

    public enum OperationType
    {
        Between = 1,
        contains = 2,
        DoesNotContain = 3,
        endswith = 4,
        eq = 5,
        gt = 6,
        gte = 7,
        In = 8,
        NotIn = 9,
        IsNull = 10,
        IsEmpty = 11,
        IsNotEmpty = 12,
        IsNotNull = 13,
        IsNotNullNorWhiteSpace = 14,
        lt = 15,
        lte = 16,
        neq = 17,
        startswith = 18
    }

}
