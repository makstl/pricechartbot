using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;

public abstract class ChartServiceBase : BackgroundService
{
    protected readonly IServiceProvider serviceProvider;
    protected readonly PriceChartOptions options;
    protected readonly ILogger logger;
    protected readonly TelegramBotClient telegramBotClient;
    protected readonly IFormatProvider formatProvider;

    public ChartServiceBase(IServiceProvider serviceProvider, PriceChartOptions options, ILogger logger)
    {
        this.serviceProvider = serviceProvider;
        this.options = options;
        this.telegramBotClient = serviceProvider.GetRequiredService<TelegramBotClient>();
        this.formatProvider = serviceProvider.GetRequiredService<IFormatProvider>();
        this.logger = logger;
    }

    int messageId = 0;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        messageId = options.MessageId;
        while (stoppingToken.IsCancellationRequested is false)
        {
            try
            {
                logger.LogInformation("Run: " + options.Symbol);
                await Run(stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
            Thread.Sleep(options.Interval * 1000 * 60);
        }
    }

    InputMediaPhoto media;
    async Task Run(CancellationToken stoppingToken)
    {
        try
        {
            var (chartStream, text) = await GetMessage();
            media = new InputMediaPhoto(InputFile.FromStream(chartStream, options.Symbol + "USD.png"));
            media.Caption = text;
            media.ParseMode = Telegram.Bot.Types.Enums.ParseMode.Html;
        }
        catch(Exception e)
		{
            logger.LogError(e, options.Symbol + ": " + e.Message);
            if (media == null)
                return;
            if (media.Caption[0] != '❌')
                media.Caption = "❌" + media.Caption + "\n\n" + e.Message;
            else
                return;
		}

        //text += "\n<a href='https://t.me/cryptopricechart'>More crypto price charts</a>";
        if (messageId > 0)
        {            
            await telegramBotClient.EditMessageMediaAsync(options.ChatId, messageId, media, cancellationToken: stoppingToken);
        }
        else
        {
            var msg = await telegramBotClient.SendPhotoAsync(options.ChatId, media.Media, caption: media.Caption, parseMode: media.ParseMode, disableNotification: true, cancellationToken: stoppingToken);
            messageId = msg.MessageId;
        }
        logger.LogInformation("Updated: " + options.Symbol + " " + DateTime.Now.ToString());
    }

    protected virtual async Task<(MemoryStream, string)> GetMessage() { throw new NotImplementedException(); }

    public async Task<List<OHLC>> GetOHLC()
    {
        var histUrl = $"https://min-api.cryptocompare.com/data/v2/histominute?fsym={options.Symbol}&tsym=USDT&limit=1440&api_key={options.ApiKey}";
        var httpClient = new HttpClient();
        var histDataStr = await httpClient.GetStringAsync(histUrl);
        var histData1 = JsonConvert.DeserializeObject<CryptoCompare.HistohourResult>(histDataStr);
        histDataStr = await httpClient.GetStringAsync(histUrl + $"&toTs={histData1.Data.TimeFrom}");
        var histData2 = JsonConvert.DeserializeObject<CryptoCompare.HistohourResult>(histDataStr);
        histDataStr = await httpClient.GetStringAsync(histUrl + $"&toTs={histData2.Data.TimeFrom}");
        var histData3 = JsonConvert.DeserializeObject<CryptoCompare.HistohourResult>(histDataStr);
        List<OHLC> result = new List<OHLC>();
        result.AddRange(histData3.Data.Data.Select(x => new OHLC { Open = x.open, High = x.high, Low = x.low, Close = x.close, TimeStamp = x.Timestamp, Volume = x.volumeto }));
        result.AddRange(histData2.Data.Data.Select(x => new OHLC { Open = x.open, High = x.high, Low = x.low, Close = x.close, TimeStamp = x.Timestamp, Volume = x.volumeto }));
        result.AddRange(histData1.Data.Data.Select(x => new OHLC { Open = x.open, High = x.high, Low = x.low, Close = x.close, TimeStamp = x.Timestamp, Volume = x.volumeto }));
        return result;
    }
    public async Task<CryptoCompare.LatestResult> GetLatest()
    {
        var histUrl = $"https://min-api.cryptocompare.com/data/blockchain/latest?fsym={options.Symbol}&api_key={options.ApiKey}";
        var httpClient = new HttpClient();
        var histDataStr = await httpClient.GetStringAsync(histUrl);
        return JsonConvert.DeserializeObject<CryptoCompare.LatestResult>(histDataStr);
    }
}
