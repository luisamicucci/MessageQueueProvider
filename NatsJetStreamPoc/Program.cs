using NATS.Client;
using NATS.Client.Internals;
using NATS.Client.JetStream;
using System.Text;

namespace NatsJetStreamPoc;

class Program
{
    static async Task Main(string[] args)
    {
        // Connect to NATS server
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = "nats://localhost:4222"; // Adjust the URL if needed

        using var connection = new ConnectionFactory().CreateConnection(opts);
        var jetStream = connection.CreateJetStreamContext();
        var jetStreamManagement = connection.CreateJetStreamManagementContext();

        // Create a stream
        var streamConfig = StreamConfiguration.Builder()
            .WithName("TDOC")
            .WithSubjects("events.*")
            .WithStorageType(StorageType.File)
            .WithDuplicateWindow(Duration.OfMinutes(1))
            .Build();

        try
        {
            jetStreamManagement.AddStream(streamConfig);
        }
        catch (NATSJetStreamException e)
        {
            Console.WriteLine($"Error adding stream: {e.Message}");
        }

        // Publish messages with the same message ID to simulate deduplication
        var msgId = Guid.NewGuid().ToString();
        var data = Encoding.UTF8.GetBytes("Hello, JetStream!");

        for (int i = 0; i < 5; i++)
        {
            var msg = new Msg("events.test", data)
            {
                Header = { ["Nats-Msg-Id"] = msgId }
            };

            await jetStream.PublishAsync(msg);
            Console.WriteLine($"Published message with ID: {msgId}");
        }

        // Subscribe to the stream
        var consumerConfig = ConsumerConfiguration.Builder()
            .WithDurable("durable-consumer")
            .Build();

        var pushSubscribeOptions = PushSubscribeOptions.Builder()
            .WithConfiguration(consumerConfig)
            .Build();

        var subscription = jetStream.PushSubscribeAsync("events.*", MessageHandler, false, pushSubscribeOptions);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        void MessageHandler(object sender, MsgHandlerEventArgs args)
        {
            var message = args.Message;
            if (message.HasHeaders && message.Header["Nats-Msg-Id"] == msgId)
            {
                Console.WriteLine($"Received message: {Encoding.UTF8.GetString(message.Data)}");
            }
            message.Ack();
        }
    }
}
