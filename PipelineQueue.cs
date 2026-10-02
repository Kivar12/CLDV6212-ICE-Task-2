using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeNChill
{
    public class PipelineQueue
    {
        private readonly TableClient _tableClient;

        public PipelineQueue()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            _tableClient = new TableClient(
                connectionString,
                "PipelineMessages");

            _tableClient.CreateIfNotExists();
        }

        [Function("PipelineQueue")]
        public async Task Run(
            [QueueTrigger("pipeline-queue", Connection = "AzureWebJobsStorage")]
            string message)
        {
            var entity = new TableEntity("Messages", Guid.NewGuid().ToString())
            {
                { "Message", message },
                { "ReceivedAt", DateTime.UtcNow }
            };

            await _tableClient.AddEntityAsync(entity);

            Console.WriteLine("Message written to Table Storage: " + message);
        }
    }
}