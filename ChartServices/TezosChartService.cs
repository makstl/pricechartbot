using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChartServices;
public class TezosChartService : ChartServiceBase
{
    public TezosChartService(IServiceProvider serviceProvider, PriceChartOptions options) :
           base(serviceProvider, options, serviceProvider.GetRequiredService<ILogger<TezosChartService>>())
    {
    }

    protected override async Task<(MemoryStream, string)> GetMessage()
    {
        var data = await GetTzktOHLC();
        var period = await GetVotingPeriod();
        var stat = await GetStat();
        var chartStream = new MemoryStream();
        var pc = new PriceChart();
        pc.GraphColor = SkiaSharp.SKColors.Blue;
        pc.ImageResource = "Tezos.jpg";
        pc.Create(data, chartStream);
        chartStream.Position = 0;
        var prev24 = data.Where(o => o.TimeStamp > data.Last().TimeStamp.AddHours(-24)).ToList();
        var delta24 = data.Last().Close - prev24.First().Close;
        string text = $@"<b>${data.Last().Close.ToString("###,###,###.00", formatProvider)} {(delta24 > 0 ? "📈" : "📉")} {(delta24 / prev24.First().Close).ToString("+0.0%;-0.0%", formatProvider)}</b> | 24h

Circulating supply: {(stat.circulatingSupply / 1_000_000M).ToString("###,###,###", formatProvider)} XTZ
Market cap: {(stat.circulatingSupply * data.Last().Close / 1_000_000M).ToString("###,###,###,###,###,###,###", formatProvider)} USD

{period.kind[0].ToString().ToUpper()}{period.kind.Substring(1)} period ends { period.endTime.ToString("MMMMM d a\\t HH:mm", formatProvider)} UTC

<i>Last update: {DateTime.UtcNow.ToString("MMMM d, HH:mm", formatProvider)} UTC</i>";

        return (chartStream, text);
    }

    async Task<List<OHLC>> GetTzktOHLC()
	{
        var histUrl = $"https://api.tzkt.io/v1/quotes?sort.desc=level&timestamp.gt={DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd'T'HH:mm:ssZ")}&limit=10000";
        var httpClient = new HttpClient();
        var histDataStr = await httpClient.GetStringAsync(histUrl);
        var histData = JsonConvert.DeserializeObject<List<Tzkt.Quote>>(histDataStr);
        histData.Reverse();
        return histData.Select(x => new OHLC { Open = x.usd, High = x.usd, Low = x.usd, Close = x.usd, TimeStamp = x.timestamp }).ToList();
    }

    async Task<Tzkt.VotingPeriod> GetVotingPeriod()
    {
        var url = $"https://api.tzkt.io/v1/voting/periods/current";
        var httpClient = new HttpClient();
        var str = await httpClient.GetStringAsync(url);
        return JsonConvert.DeserializeObject<Tzkt.VotingPeriod>(str);
    }

    async Task<Tzkt.Stat> GetStat()
    {
        var url = $"https://api.tzkt.io/v1/statistics?sort.desc=level&limit=1";
        var httpClient = new HttpClient();
        var str = await httpClient.GetStringAsync(url);
        return JsonConvert.DeserializeObject<List<Tzkt.Stat>>(str)[0];
    }
}
