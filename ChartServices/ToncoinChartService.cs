using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChartServices;

public class ToncoinChartService : ChartServiceBase
{
    public ToncoinChartService(IServiceProvider serviceProvider, PriceChartOptions options) :
              base(serviceProvider, options, serviceProvider.GetRequiredService<ILogger<ToncoinChartService>>())
    {
    }

    protected override async Task<(MemoryStream, string)> GetMessage()
    {
        var data = await GetGateOHLC();
        
        var chartStream = new MemoryStream();
        var pc = new PriceChart();
        pc.GraphColor = new SkiaSharp.SKColor(0xFF0088CC);
        pc.ImageResource = "Toncoin.jpg";
        pc.Create(data, chartStream);
        chartStream.Position = 0;
        var prev24 = data.Where(o => o.TimeStamp > data.Last().TimeStamp.AddHours(-24)).ToList();
        var delta24 = data.Last().Close - prev24.First().Close;
        string text = $@"<b>${data.Last().Close.ToString("###,###,###.00", formatProvider)} {(delta24 > 0 ? "📈" : "📉")} {(delta24 / prev24.First().Close).ToString("+0.0%;-0.0%", formatProvider)}</b> | 24h

<i>Last update: {DateTime.UtcNow.ToString("MMMM d, HH:mm", formatProvider)} UTC</i>";

        return (chartStream, text);
    }

    async Task<List<OHLC>> GetGateOHLC()
    {
        var histUrl = $"https://api.gateio.ws/api/v4/spot/candlesticks?currency_pair=TON_USDT&interval=5m&limit=864";
        var httpClient = new HttpClient();
        var histDataStr = await httpClient.GetStringAsync(histUrl);
        var histData = JsonConvert.DeserializeObject<List<List<string>>>(histDataStr);
        return histData.Select(x => new OHLC { Open = decimal.Parse(x[5]), High = decimal.Parse(x[3]), Low = decimal.Parse(x[4]), Close = decimal.Parse(x[2]), TimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(int.Parse(x[0])) }).ToList();
    }
}
