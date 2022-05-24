using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Topics
{
    public class Producer : BackgroundService
    {
        private readonly ILogger<Producer> _logger;

        public Producer(ILogger<Producer> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int idx = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                
                var config = new ProducerConfig
                {
                    BootstrapServers = "localhost:9092",
                    ClientId = Dns.GetHostName()
                };

                using var producer = new ProducerBuilder<string, string>(config).Build();
                await producer.ProduceAsync(Topics.Messages, new Message<string, string>()
                {
                    Key = (idx + 1).ToString(),
                    Value = Guid.NewGuid().ToString()
                }, stoppingToken);
                idx++;
                _logger.LogInformation("Message sended, " +  (idx + 1).ToString());
            }
        }
    }
}