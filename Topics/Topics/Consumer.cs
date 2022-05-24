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
    public class Consumer : BackgroundService
    {
        private readonly ILogger<Consumer> _logger;
        private readonly IConsumer<string, string> _consumer;

        public Consumer(ILogger<Consumer> logger)
        {
            _logger = logger;
            var conf = new ConsumerConfig
            {
                GroupId = "st_consumer_group",
                BootstrapServers = "localhost:9092",
                ClientId = Dns.GetHostName(),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<string, string>(conf).Build();
        }

        public override async Task StartAsync(CancellationToken token)
        {
            _consumer.Subscribe(Topics.Messages);
            await base.StartAsync(token);
        }
        
        public override async Task StopAsync(CancellationToken token)
        {
            _consumer.Close();
            await base.StopAsync(token);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                Console.WriteLine("Received {0} {1}", consumeResult.Key, consumeResult.Message);
                await Task.CompletedTask;
            }
        }
    }
}