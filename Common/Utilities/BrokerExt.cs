using System;
using Binance.Net.Enums;

namespace Common.Utilities
{
    public static class BrokerExt
    {

        public static int ToMin(this KlineInterval input)
        {
            return input switch
            {
                KlineInterval.OneMinute => 1,
                KlineInterval.ThreeMinutes => 3,
                KlineInterval.FiveMinutes => 5,
                KlineInterval.FifteenMinutes => 15,
                KlineInterval.ThirtyMinutes => 30,
                KlineInterval.OneHour => 60,
                KlineInterval.TwoHour => 120,
                KlineInterval.FourHour => 240,
                KlineInterval.SixHour => 360,
                KlineInterval.EightHour => 480,
                KlineInterval.TwelveHour => 720,
                KlineInterval.OneDay => 1440,
                KlineInterval.ThreeDay => 4320,
                KlineInterval.OneWeek => 10080,
                KlineInterval.OneMonth => 43200,
                _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
            };
        }

        public static KlineInterval ToInterval(this int input)
        {
            return input switch
            {
                1 => KlineInterval.OneMinute,
                3 => KlineInterval.ThreeMinutes,
                5 => KlineInterval.FiveMinutes,
                15 => KlineInterval.FifteenMinutes,
                30 => KlineInterval.ThirtyMinutes,
                60 => KlineInterval.OneHour,
                120 => KlineInterval.TwoHour,
                240 => KlineInterval.FourHour,
                360 => KlineInterval.SixHour,
                480 => KlineInterval.EightHour,
                720 => KlineInterval.TwelveHour,
                1440 => KlineInterval.OneDay,
                4320 => KlineInterval.ThreeDay,
                10080 => KlineInterval.OneWeek,
                43200 => KlineInterval.OneMonth,
                _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
            };
        }
    }
}
