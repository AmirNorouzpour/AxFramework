using System;
using AutoMapper;
using Entities.MasterSignal;
using WebFramework.Api;

namespace API.Models
{
    public class AxPositionDto : BaseDto<AxPositionDto, AxPosition, int>
    {
        public string Symbol { get; set; }
        public string EnterPrice { get; set; }
        public string StopLoss { get; set; }
        public string Targets { get; set; }
        public decimal Max { get; set; }
        public decimal ProfitPercent { get; set; }
        public string DateTime { get; set; }
        public string TimeFrame { get; set; }
        public decimal Price { get; set; }
        public string Risk { get; set; }
        public string Capital { get; set; }
        public decimal Leverage { get; set; }
        public string SuggestedLeverage { get; set; }
        public string Status { get; set; }
        public bool IsFree { get; set; }
        public bool? StopMoved { get; set; }
        public string Side { get; set; }
        public string Result { get; set; }
    }

}
