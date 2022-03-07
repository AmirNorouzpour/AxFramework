using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Entities.Framework;
using FluentValidation;

namespace Entities.MasterSignal
{
    public class AxSignal : BaseEntity
    {
        public string Symbol { get; set; }
        public string Side { get; set; }
        public DateTime DateTime { get; set; }
        public string TimeFrame { get; set; }
        public decimal EnterPrice { get; set; }
    }

    public class AxSignalValidator : AbstractValidator<AxSignal>
    {
    }


    public class AxPosition : BaseEntity
    {
        public int AxSignalId { get; set; }
        [ForeignKey("AxSignalId")]
        public AxSignal AxSignal { get; set; }
        public string Symbol { get; set; }
        public decimal EnterPrice { get; set; }
        public decimal StopLoss { get; set; }
        public string Targets { get; set; }
        [NotMapped]
        public List<decimal> TargetsList => Targets.Split(",").ToList().Select(decimal.Parse).ToList();
        public decimal Max { get; set; }
        public decimal ProfitPercent { get; set; }
        public DateTime DateTime { get; set; }
        [NotMapped]
        public DateTime LastUpdate { get; set; }
        public string TimeFrame { get; set; }
        public decimal Price { get; set; }
        public string Risk { get; set; }
        public string Capital { get; set; }
        public decimal Leverage { get; set; }
        public string SuggestedLeverage { get; set; }
        public PositionStatus Status { get; set; }
        public bool IsFree { get; set; }
        public int ReachedTarget { get; set; }
        public bool StopMoved { get; set; }
        public PositionSide Side { get; set; }
        public PositionResult Result { get; set; }

        public bool SetPrice(decimal price)
        {

            if (Status == PositionStatus.NotStarted)
            {
                Price = price;
                if (Side == PositionSide.Long && price <= EnterPrice || Side == PositionSide.Short && price >= EnterPrice)
                {
                    Status = PositionStatus.Started;
                    //todo:Push Started
                }
            }
            else if (Status == PositionStatus.Started)
            {
                Price = price;
                ProfitPercent = Side == PositionSide.Long ? (price - EnterPrice) * 100 / EnterPrice * Leverage : (EnterPrice - price) * 100 / EnterPrice * Leverage;
                Max = Max < ProfitPercent ? ProfitPercent : Max;
                var t2 = TargetsList.Skip(1).FirstOrDefault();

                if ((Side == PositionSide.Long && price <= StopLoss || Side == PositionSide.Short && price >= StopLoss) && Result != PositionResult.Stopped)
                {
                    Result = PositionResult.Stopped;
                    Status = PositionStatus.Closed;
                    //todo:Push Stopped
                    return true;
                }

                var i = 0;
                foreach (var target in TargetsList)
                {
                    if (Side == PositionSide.Long && Max >= target || Side == PositionSide.Short && Max <= target)
                        i++;
                }

                if (i > ReachedTarget)
                {
                    if (i == TargetsList.Count)
                    {
                        Result = PositionResult.FullTarget;
                        Status = PositionStatus.Closed;
                    }

                    var text = $"target {i} reached. {Symbol} is {TargetsList[i - 1]}.";
                    //todo:Push target reached
                    return true;
                }

                if ((Side == PositionSide.Long && price >= t2 || Side == PositionSide.Short && price <= t2) && !StopMoved)
                {
                    StopLoss = EnterPrice;
                    StopMoved = true;
                    //todo:Push StopMoved
                    return true;
                }
            }

            return false;
        }
    }

    public class AxPositionValidator : AbstractValidator<AxPosition>
    {
    }

    public class AxUserSetting : BaseEntity
    {
        public bool SignalNotify { get; set; }
        public bool NewsNotify { get; set; }
        public bool AnalysisNotify { get; set; }
    }

    public class AxUserSettingValidator : AbstractValidator<AxUserSetting>
    {
    }

    public class AnalysisMsg : BaseEntity
    {
        public string Content { get; set; }
        public bool Title { get; set; }
        public string Tags { get; set; }
        public string Side { get; set; }
        public int Views { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public DateTime DateTime { get; set; }
        public AnalysisMsgType Type { get; set; }
    }
    public class AnalysisMsgValidator : AbstractValidator<AnalysisMsg>
    {
    }

    public class Transaction : BaseEntity
    {
        public string Title { get; set; }
        public Duration Duration { get; set; }
        public TransactionStatus Status { get; set; }
        public string TransactionId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public DateTime DateTime { get; set; }
    }
    public class TransactionValidator : AbstractValidator<Transaction>
    {
    }

    public enum AnalysisMsgType
    {
        Msg = 1,
        Analysis = 2
    }

    public enum PositionStatus
    {
        NotStarted = 0,
        Canceled = 1,
        Started = 2,
        Closed = 3
    }

    public enum TransactionStatus
    {
        Submitted = 0,
        Rejected = 1,
        Accepted = 2
    }

    public enum Duration
    {
        OneMonth = 30,
        ThreeMonth = 90,
        SixMonth = 180,
        OneYear = 365,
        TwoYear = 720
    }

    public enum PositionSide
    {
        Long,
        Short
    }
    public enum PositionResult
    {
        Profited = 1,
        FullTarget = 2,
        Stopped = 3,
        //Canceled = 4,
    }
}
