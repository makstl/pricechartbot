﻿namespace CryptoCompare;

public class HistohourResult
{
    public string Response { get; set; }
    public string Message { get; set; }
    public bool HasWarning { get; set; }
    public int Type { get; set; }
    public ResultData Data { get; set; }
}
public class ResultData
{
    public bool Aggregated { get; set; }
    public int TimeFrom { get; set; }
    public int TimeTo { get; set; }
    public List<Datum> Data { get; set; }
}
public class Datum
{
    public DateTime Timestamp => DateTimeOffset.FromUnixTimeSeconds(time).DateTime;
    public int time { get; set; }
    public decimal high { get; set; }
    public decimal low { get; set; }
    public decimal open { get; set; }
    public decimal volumefrom { get; set; }
    public decimal volumeto { get; set; }
    public decimal close { get; set; }
    public string conversionType { get; set; }
    public string conversionSymbol { get; set; }
}
public class DataLatest
{
    public long id { get; set; }
    public int time { get; set; }
    public string symbol { get; set; }
    public string partner_symbol { get; set; }
    public long zero_balance_addresses_all_time { get; set; }
    public long unique_addresses_all_time { get; set; }
    public long new_addresses { get; set; }
    public long active_addresses { get; set; }
    public double average_transaction_value { get; set; }
    public long block_height { get; set; }
    public double hashrate { get; set; }
    public double difficulty { get; set; }
    public double block_time { get; set; }
    public long block_size { get; set; }
    public decimal current_supply { get; set; }
    public long transaction_count { get; set; }
    public long transaction_count_all_time { get; set; }
    public long large_transaction_count { get; set; }
}

public class LatestResult
{
    public string Response { get; set; }
    public string Message { get; set; }
    public bool HasWarning { get; set; }
    public int Type { get; set; }
    public DataLatest Data { get; set; }
}
