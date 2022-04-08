using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CoinMarketCap.WebApi.Services
{
    public abstract class PeriodicService : BackgroundService
    {
        protected int SleepTime;
        protected readonly ILogger<PeriodicService> Logger;

        protected PeriodicService(int sleepTime,
            ILogger<PeriodicService> logger)
        {
            SleepTime = sleepTime;
            Logger = logger;
        }

        private bool _started;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogDebug($"PeriodicService '{GetName()}' is starting.");

            stoppingToken.Register(() => Logger.LogDebug($"Background task '{GetName()}' is stopping."));
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(Run, stoppingToken);
                await Task.Delay(SleepTime, stoppingToken);
            }

            Logger.LogDebug($"Background task '{GetName()}' is stopping.");
        }

        private async Task Run()
        {
            if (_started)
                return;

            _started = true;

            try
            {
                await Do();
            }
            catch (Exception e)
            {
                Logger.LogError($"Background task '{GetName()}' has error: {e}");
            }
            finally
            {
                _started = false;
            }
        }

        protected virtual async Task Do()
        {
        }
    }
}
