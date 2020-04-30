﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Framework.Reports
{
    public class NewReport : BaseEntity
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public int TakeSize { get; set; }
        public ICollection<ColumnReport> Columns { get; set; }
        public ICollection<OrderByReport> Orders { get; set; }
    }

    public class ColumnReport : BaseEntity
    {
        public int FieldId { get; set; }
        public string Name { get; set; }
        public int ReportId { get; set; }
        [ForeignKey("ReportId")]
        public NewReport Report { get; set; }
    }

    public class OrderByReport : BaseEntity
    {
        public int OrderIndex { get; set; }
        public string Column { get; set; }
        public OrderByType OrderByType { get; set; }
        public int ReportId { get; set; }
        [ForeignKey("ReportId")]
        public NewReport Report { get; set; }
    }

    public class Filter : BaseEntity
    {
        public string Property { get; set; }
        public OperationType Operation { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public ConnectorType Connector { get; set; }
        public int? ReportId { get; set; }
        [ForeignKey("ReportId")]
        public NewReport Report { get; set; }
        public bool IsCalculation { get; set; }
        public bool IsActive { get; set; } = true;

        //public string Type { get; set; }
    }

    public enum OrderByType
    {
        Asc,
        Desc
    }

    public enum ConnectorType
    {
        And,
        Or
    }
}