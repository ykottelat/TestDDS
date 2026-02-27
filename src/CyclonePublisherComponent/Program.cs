using System;
using System.Threading;
using System.Threading.Tasks;
using CycloneDDS.Core;
using CycloneDDS.Runtime;
using CycloneDDS.Schema;

namespace HelloWorld
{
    [DdsTopic("HelloWorldTopic")]
    public partial struct HelloWorldMessage
    {
        [DdsKey]
        public int Id;
        [DdsManaged]
        public string Message;
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting CycloneDDS.NET Hello World...");

            // Create a participant
            using var participant = new DdsParticipant();

            // Create a writer - topic name "HelloWorldTopic" automatically used from [DdsTopic] attribute
            using var writer = new DdsWriter<HelloWorldMessage>(participant);

            // Create a reader - topic name "HelloWorldTopic" automatically used from [DdsTopic] attribute
            using var reader = new DdsReader<HelloWorldMessage>(participant);

            using var readCancellation = new CancellationTokenSource();

            // Publisher thread
            var publishTask = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(500);
                    var msg = new HelloWorldMessage { Id = i, Message = $"Hello World {i}" };
                    Console.WriteLine($"Writing: {msg.Message}");
                    writer.Write(msg);
                }
            });

            // Reader thread
            var readTask = Task.Run(async () =>
            {
                Console.WriteLine("Waiting for data...");
                HelloWorldMessage lastPrintedMessage = default;
                var hasPrintedMessage = false;

                while (!readCancellation.Token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(250, readCancellation.Token);
                        using var samples = reader.Read();
                        HelloWorldMessage latestMessage = default;
                        var hasSample = false;

                        foreach (var sample in samples)
                        {
                            latestMessage = sample.Data;
                            hasSample = true;
                        }

                        if (hasSample &&
                            (!hasPrintedMessage ||
                             latestMessage.Id != lastPrintedMessage.Id ||
                             latestMessage.Message != lastPrintedMessage.Message))
                        {
                            Console.WriteLine($"Received latest: [{latestMessage.Id}] {latestMessage.Message}");
                            lastPrintedMessage = latestMessage;
                            hasPrintedMessage = true;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected during shutdown.
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Read error: {ex.Message}");
                    }
                }
            }, readCancellation.Token);

            await publishTask;
            readCancellation.Cancel();
            await readTask;

            Console.WriteLine("Done.");
        }
    }
}
