using System;
using System.Collections.Generic;

namespace API.Models
{
    public class GlobalFGI
    {
        public long total_market_cap_usd { get; set; }
        public long total_24h_volume_usd { get; set; }
        public double bitcoin_percentage_of_market_cap { get; set; }
        public long active_currencies { get; set; }
        public long active_assets { get; set; }
        public long active_markets { get; set; }
        public long last_updated { get; set; }

    }

    public class GlobalSymbol
    {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public string rank { get; set; }
        public string price_usd { get; set; }
        public string price_btc { get; set; }
        public string _24h_volume_usd { get; set; }
        public string market_cap_usd { get; set; }
        public string available_supply { get; set; }
        public string total_supply { get; set; }
        public string max_supply { get; set; }
        public string percent_change_1h { get; set; }
        public string percent_change_24h { get; set; }
        public string percent_change_7d { get; set; }
        public string last_updated { get; set; }
    }

    public class GlobalResult
    {
        public string BTCPercent24h;
        public string BTCPrice;
        public string ETHPrice;
        public string ETHPercent24h;
        public string BitcoinPercentageOfMarketCap { get; set; }
        public string TotalMarketCap { get; set; }
        public string TotalVolume24h { get; set; }
        public long LastUpdated { get; set; }
        public string Dom { get; set; }
        public List<FGModelRes> Fng { get; set; }
    }

    public class FG
    {
        public string value { get; set; }
        public string value_classification { get; set; }
        public string timestamp { get; set; }
        public long time_until_update { get; set; }
    }

    public class Metadata
    {
        public object error { get; set; }
    }

    public class FGModel
    {
        public string name { get; set; }
        public List<FG> data { get; set; }
        public Metadata metadata { get; set; }
    }

    public class FGModelRes
    {
        public string date { get; set; }
        public string value { get; set; }
        public string name { get; set; }
    }
    public class Roi
    {
        public double times { get; set; }
        public string currency { get; set; }
        public double percentage { get; set; }
    }

    public class DomModel
    {
        public string id { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public double current_price { get; set; }
        public decimal market_cap { get; set; }
        public int market_cap_rank { get; set; }
        public long? fully_diluted_valuation { get; set; }
        public object total_volume { get; set; }
        public double high_24h { get; set; }
        public double low_24h { get; set; }
        public double price_change_24h { get; set; }
        public double price_change_percentage_24h { get; set; }
        public object market_cap_change_24h { get; set; }
        public double market_cap_change_percentage_24h { get; set; }
        public double circulating_supply { get; set; }
        public double? total_supply { get; set; }
        public double? max_supply { get; set; }
        public double ath { get; set; }
        public double ath_change_percentage { get; set; }
        public DateTime ath_date { get; set; }
        public double atl { get; set; }
        public double atl_change_percentage { get; set; }
        public DateTime atl_date { get; set; }
        public Roi roi { get; set; }
        public DateTime last_updated { get; set; }
    }

}
