using System;
using System.ComponentModel.DataAnnotations.Schema;
using Binance.Net.Enums;
using Entities.Framework;
using FluentValidation;

namespace Entities.MasterSignal
{
    public class UserData : BaseEntity
    {
        public string MobileNumber { get; set; }
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
        public string PhrasePassword { get; set; }
        public DateTime ExpireDate { get; set; }
        public decimal InitBalance { get; set; }
        public decimal Balance { get; set; }
    }

    public class UserDataValidator : AbstractValidator<UserData>
    {
    }

    public class AxPositionLog : BaseEntity
    {
        public string Symbol { get; set; }
        public decimal EnterAveragePrice { get; set; }
        public decimal ExitAveragePrice { get; set; }
        [NotMapped]
        public decimal TempProfit { get; set; }
        public decimal Quantity { get; set; }
        public OrderSide Side { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitPercent { get; set; }
        public decimal Commission { get; set; }
        [NotMapped]
        public bool InProgress { get; set; }
        public DateTime EnterTime { get; set; }
        public DateTime ExitTime { get; set; }
    }
    public class AxPositionLogValidator : AbstractValidator<AxPositionLog>
    {
    }

}
