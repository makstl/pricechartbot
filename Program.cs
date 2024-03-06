using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using Telegram.Bot;

CreateHostBuilder(args).Build().Run();

IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) => {
                builder.AddJsonFile("Settings.json");
                builder.AddJsonFile($"Settings.{context.HostingEnvironment.EnvironmentName}.json", true);
                builder.AddJsonFile("Settings.Local.json", true);
                builder.AddEnvironmentVariables();
                builder.AddCommandLine(args);
            })
            .ConfigureServices((context, services) => {
                services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug).AddConsole().AddFile("Logs/log-{Date}.txt"));
                services.Configure<TelegramOptions>(context.Configuration.GetSection("Telegram"));
                services.Configure<PriceChartOptionsList>(context.Configuration.GetSection("PriceChartOptions"));
                
                services.AddLogging(builder => builder.AddConsole());
                services.AddSingleton<IFormatProvider>(CultureInfo.GetCultureInfo("en"));
                services.AddSingleton(provider => new TelegramBotClient(provider.GetRequiredService<IOptions<TelegramOptions>>().Value.BotSecret));

				services.AddHostedService(provider => new ChartServices.BitcoinChartService(provider, provider.GetRequiredService<IOptions<PriceChartOptionsList>>().Value.Bitcoin));
				services.AddHostedService(provider => new ChartServices.EthereumChartService(provider, provider.GetRequiredService<IOptions<PriceChartOptionsList>>().Value.Ethereum));
				services.AddHostedService(provider => new ChartServices.TezosChartService(provider, provider.GetRequiredService<IOptions<PriceChartOptionsList>>().Value.Tezos));
				services.AddHostedService(provider => new ChartServices.ToncoinChartService(provider, provider.GetRequiredService<IOptions<PriceChartOptionsList>>().Value.Toncoin));
				services.AddHostedService(provider => new ChartServices.MinaChartService(provider, provider.GetRequiredService<IOptions<PriceChartOptionsList>>().Value.Mina));
            });