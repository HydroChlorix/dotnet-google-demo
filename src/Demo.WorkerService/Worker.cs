using Demo.WorkerService.Options;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Options;

namespace Demo.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<PubSubOption> _options;
        private SubscriberClient subscriber;

        public Worker(ILogger<Worker> logger, IOptions<PubSubOption> options)
        {
            _logger = logger;
            _options = options;
        }
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Start at {DateTimeOffset.Now}");

            // Pull messages from the subscription using SubscriberClient.
            SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(_options.Value.ProjcetId, _options.Value.SubscriptionId);
            subscriber = await SubscriberClient.CreateAsync(subscriptionName);

            await base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Start the subscriber listening for messages.
            await subscriber.StartAsync((msg, cancellationToken) =>
            {
                Console.WriteLine($"Received message {msg.MessageId} published at {msg.PublishTime.ToDateTime()}");
                Console.WriteLine($"Text: '{msg.Data.ToStringUtf8()}'");

                foreach (var attr in msg.Attributes.ToDictionary())
                {
                    Console.WriteLine($"Attribute: '{attr.Key}'");
                    Console.WriteLine($"Value: '{attr.Value}'");
                }

                return Task.FromResult(SubscriberClient.Reply.Ack);
            });

        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"Stop at {DateTimeOffset.Now}");

            await subscriber.StopAsync(cancellationToken);

            await base.StopAsync(cancellationToken);
        }
    }
}
