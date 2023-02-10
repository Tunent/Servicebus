using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Models;
using Azure.Storage.Blobs.Specialized;
using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace BillingService
{
    public class BillingQuueFunction
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

            await UploadBlobFileAsync(containerClient, orderPlaced.OrderId);

            OrderBilled orderBilled = new OrderBilled() 
            {
                Id = orderPlaced.OrderId 
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
