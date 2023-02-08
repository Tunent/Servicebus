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

namespace BillingService
{
    public static class BillingQuueFunction
    {
        [FunctionName("BillingQueue")]
        [return: ServiceBus("shippingQueue", Connection = "ServiceBusConnectionString")]
        public static async Task Run([ServiceBusTrigger("billingQueue", Connection = "ServiceBusConnectionString")] 
            OrderPlaced orderPlaced, ILogger logger ,[Blob("billing", FileAccess.Read, Connection = "AzureWebJobsStorage")]
            BlobContainerClient containerClient)
        {
            logger.LogInformation($"BillingFunction queue trigger function processed message");
         

            logger.LogInformation($"BillingQueue function {orderPlaced}");

            string blobName = orderPlaced.OrderId + ".txt";

          
            BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient(blobName);
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(orderPlaced.OrderId));
            await blockBlobClient.UploadAsync(mStream);
        }
    }
}
