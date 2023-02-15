using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BillingService
{
    public class BillingQueueFunction
    {
        [FunctionName("BillingQueue")]
        [return: ServiceBus("trevortopic", Connection = "ServiceBusConnectionString")]
        public static async Task<ServiceBusMessage> Run(
            [ServiceBusTrigger(queueName:"billingqueue", Connection = "ServiceBusConnectionString")]
            OrderPlaced orderPlaced, ILogger logger,
            [Blob("billing", FileAccess.Read, Connection = "AzureWebJobsStorage")]
            BlobContainerClient containerClient)
        {
            logger.LogInformation($"BillingFunction queue trigger function processed message");

            if (orderPlaced == null)
            {
                logger.LogInformation("orderplaced is empty");

            }
            await UploadBlobFileAsync(containerClient, orderPlaced.OrderId);

            OrderBilled orderBilled = new OrderBilled()
            {
                OrderId = orderPlaced.OrderId
            };
            logger.LogInformation($"BillingQueue function {orderPlaced}");

            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(orderBilled));
            serviceBusMessage.ApplicationProperties.Add("Type", "OrderBilled");

            return serviceBusMessage;
        }

        private static async Task UploadBlobFileAsync(BlobContainerClient containerClient, string orderId)
        {
            string blobName = orderId + ".txt";
            BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(blobName);
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
            await blockBlobClient.UploadAsync(mStream);
        }
    }
}

