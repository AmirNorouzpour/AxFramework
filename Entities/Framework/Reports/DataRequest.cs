using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Common;
using Common.Exception;
using ExpressionBuilder.Common;
using ExpressionBuilder.Generics;
using ExpressionBuilder.Interfaces;
using ExpressionBuilder.Operations;
using FluentValidation;
//using Z.Expressions;

namespace Entities.Framework.Reports
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
        public Connector Connector { get; set; }
        public int? ReportId { get; set; }
        [ForeignKey("ReportId")]
        public Report Report { get; set; }
        public bool IsCalculation { get; set; }

        //public string Type { get; set; }
    }

    public class AxFilterValidator : AbstractValidator<AxFilter> { }

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
    public static class OperationExt
    {
        public static IOperation GetOperation(this OperationType type)
        {
            switch (type)
            {
                case OperationType.Between:
                    return Operation.Between;
                case OperationType.contains:
                    return Operation.Contains;
                case OperationType.DoesNotContain:
                    return Operation.DoesNotContain;
                case OperationType.endswith:
                    return Operation.EndsWith;
                case OperationType.eq:
                    return Operation.EqualTo;
                case OperationType.gt:
                    return Operation.GreaterThan;
                case OperationType.gte:
                    return Operation.GreaterThanOrEqualTo;
                case OperationType.In:
                    return Operation.In;
                case OperationType.NotIn:
                    return Operation.NotIn;
                case OperationType.IsNull:
                    return Operation.IsNull;
                case OperationType.IsEmpty:
                    return Operation.IsEmpty;
                case OperationType.IsNotEmpty:
                    return Operation.IsNotEmpty;
                case OperationType.IsNotNull:
                    return Operation.IsNotNull;
                case OperationType.IsNotNullNorWhiteSpace:
                    return Operation.IsNotNullNorWhiteSpace;
                case OperationType.lt:
                    return Operation.LessThan;
                case OperationType.lte:
                    return Operation.LessThanOrEqualTo;
                case OperationType.neq:
                    return Operation.NotEqualTo;
                case OperationType.startswith:
                    return Operation.StartsWith;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static Filter<TClass> GetFilter<TClass>(this DataRequest request) where TClass : class
        {
            if (request.Filters != null)
            {
                var predicate = new Filter<TClass>();
                foreach (var requestFilter in request.Filters)
                {
                    var entityType = typeof(TClass);
                    var property = entityType.GetProperties().FirstOrDefault(x => x.Name.Equals(requestFilter.Property, StringComparison.InvariantCultureIgnoreCase));
                    if (property == null)
                        throw new AppException(ApiResultStatusCode.BadRequest, $"{requestFilter.Property} not found in {entityType.Name}");

                    var type = Nullable.GetUnderlyingType(property.GetType()) ?? property.PropertyType;

                    if (requestFilter.IsCalculation)
                    {
                        if (!string.IsNullOrWhiteSpace(requestFilter.Value1))
                            requestFilter.Value1 = CodeUtil.Execute<string>(requestFilter.Value1);
                        if (!string.IsNullOrWhiteSpace(requestFilter.Value2))
                            requestFilter.Value2 = CodeUtil.Execute<string>(requestFilter.Value2);
                    }

                    var typeConverter = TypeDescriptor.GetConverter(type);
                    var value1 = typeConverter.ConvertFromString(requestFilter.Value1);
                    object value2 = null;
                    if (!string.IsNullOrWhiteSpace(requestFilter.Value2))
                        value2 = typeConverter.ConvertFromString(requestFilter.Value2);
                    predicate.By(requestFilter.Property, requestFilter.Operation.GetOperation(), value1, value2, requestFilter.Connector);


                    //if (requestFilter.Type == "Decimal")
                    //{
                    //    Decimal fieldValue = Convert.ToDecimal(requestFilter.Value1);
                    //    predicate.By(requestFilter.Property, Operation.ByName(requestFilter.Value1), fieldValue, default(Decimal), requestFilter.Connector);
                    //} else if (criteria.Type == "DateTime")
                    //{
                    //    DateTimeOffset fieldValue = new DateTimeOffset(DateTime.Parse(criteria.FieldValue.ToString()));
                    //    predicate.By(item, Operation.ByName(criteria.OperationName), fieldValue, default(DateTimeOffset?), criteria.Connector);
                    //} else if (criteria.Type == "Int16")
                    //{
                    //    Int16 fieldValue = Convert.ToInt16(criteria.FieldValue);
                    //    predicate.By(item, Operation.ByName(criteria.OperationName), fieldValue, default(Int16), criteria.Connector);
                    //}
                    //else
                    //{
                    //    // String
                    //    predicate.By(item, Operation.ByName(criteria.OperationName), criteria.FieldValue, null, criteria.Connector);
                    //}
                }
                return predicate;
            }

            return null;
        }

        public static bool HasMoreThanThreeElements<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Count() > 3;
        }

    }
}
