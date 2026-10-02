using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text;

namespace CoffeeNChill
{
    public class PipelineHttp
    {
        private readonly QueueClient _queueClient;

        public PipelineHttp()
        {
            string connectionString =
                Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            _queueClient = new QueueClient(connectionString, "pipeline-queue");

            _queueClient.CreateIfNotExists();
        }

        [Function("PipelineHttp")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "pipeline")]
            HttpRequestData req)
        {
            string message;

            using (StreamReader reader = new StreamReader(req.Body))
            {
                message = await reader.ReadToEndAsync();
            }

            await _queueClient.SendMessageAsync(message);

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteStringAsync(
                "Message successfully sent to the queue: " + message);

            return response;
        }
    }
}