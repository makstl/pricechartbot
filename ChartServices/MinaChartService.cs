using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChartServices;
public class MinaChartService : ChartServiceBase
{
	public MinaChartService(IServiceProvider serviceProvider, PriceChartOptions options) :
	   base(serviceProvider, options, serviceProvider.GetRequiredService<ILogger<MinaChartService>>())
	{
	}

    protected override async Task<(MemoryStream, string)> GetMessage()
    {
        var data = await GetOHLC();
        //var latest = await GetLatest();
        var chartStream = new MemoryStream();
        var pc = new PriceChart();
        pc.ImageResource = "Mina.jpg";
        pc.GraphColor = new SkiaSharp.SKColor(0xFF1b2d4d);
        pc.Create(data, chartStream);
        chartStream.Position = 0;
        var prev24 = data.Where(o => o.TimeStamp > data.Last().TimeStamp.AddHours(-24)).ToList();
        var delta24 = data.Last().Close - prev24.First().Close;
        string text = $@"<b>${data.Last().Close.ToString("###,###,###.00", formatProvider)} {(delta24 > 0 ? "📈" : "📉")} {(delta24 / prev24.First().Close).ToString("+0.0%;-0.0%", formatProvider)}</b> | 24h

Volume 24h: {prev24.Sum(o => o.Volume).ToString("###,###,###,###,###,###,###", formatProvider)} USD

<i>Last update: {DateTime.UtcNow.ToString("MMMM d, HH:mm", formatProvider)} UTC</i>";

        return (chartStream, text);
    }
}
