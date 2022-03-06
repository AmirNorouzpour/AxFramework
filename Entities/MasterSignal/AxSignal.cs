using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Entities.Framework;

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
        public string TimeFrame { get; set; }
        public decimal Price { get; set; }
        public string Risk { get; set; }
        public string Capital { get; set; }
        public decimal Leverage { get; set; }
        public string SuggestedLeverage { get; set; }
        public PositionStatus Status { get; set; }
        public bool IsFree { get; set; }
    }

    public class AxUserSetting : BaseEntity
    {
        public bool SignalNotify { get; set; }
        public bool NewsNotify { get; set; }
        public bool AnalysisNotify { get; set; }
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
}
