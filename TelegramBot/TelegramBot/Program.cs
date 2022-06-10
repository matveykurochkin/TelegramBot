using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using TelegramBot.Configuration;

namespace TelegramBot;

static class Program
{

    static async Task Main(string[] args)
    {
        var logger = LogManager.GetCurrentClassLogger();
        logger.Info("Starting bot");
        try
        {
            using IHost host = Host.CreateDefaultBuilder(args)
            .UseNLog()
            .ConfigureServices((context, services)=>
            {
                services.AddOptions<TelegramOptions>()
                .Bind(context.Configuration.GetSection("Telegram")).Validate(to => to.EsureValid(), "Configuration for telegram bot is invalid. Please specify token");
            })
            .UseWindowsService(options =>
            {
                 options.ServiceName = ".NET Telegram Bot test service";
            })
            .ConfigureServices(services =>
            {
                services.AddHostedService<BotHostedService>();
            })
            .Build();

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error while bot working");
            logger.Info("Service stopped with error");
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}
