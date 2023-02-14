using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Models;
using Azure.Storage.Blobs.Specialized;
using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Azure.Data.Tables;
using Azure;

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
            BlobContainerClient containerClient,
             [Table("servicebustable", Connection = "BlobStorageConnectionString")]
            TableClient tableClient)
        {


            logger.LogInformation($"BillingFunction queue trigger function processed message");


            if(orderPlaced == null)
            {
                logger.LogInformation("orderplaced is empty");
               
            }
            await UploadBlobFileAsync(containerClient, orderPlaced.OrderId);
       

            await SaveOrder2(tableClient, logger, orderPlaced.OrderId.ToString());

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



        [FunctionName("SaveOrder")]
        [return: Table("servicebustable", Connection = "BlobStorageConnectionString")]
        public static Order SaveOrder([HttpTrigger] dynamic input, ILogger log)
        {
            log.LogInformation($"C# http Table trigger function processed: {input}");
            return new Order { PartitionKey = "TestKey", RowKey = "TestRowkey"};

        }

        public class Order2 : ITableEntity
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
            public string OrderId { get; set; }
            public bool IsOrdered { get; set; }
            public bool IsBilled { get; set; }

        }

        [FunctionName("SaveOrder2")]
        public static async Task SaveOrder2(
         [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "order/{orderId}")]

        TableClient tableClient, ILogger log, string orderId)
        {
            //tableClient = new TableClient("BlobStorageConnectionString", "servicebustable");
            Order2 tester = new Order2()
            {
                PartitionKey = "anotherTest",
                OrderId = orderId,
                RowKey = orderId,
                IsBilled = true,

            };
            //var result = await tableClient.GetEntityAsync(partitionKey: tester.PartitionKey.ToString(), 
            //    rowKey: tester.RowKey.ToString());

            //await tableClient.AddEntityAsync(tester);
            //AsyncPageable<Order2> queryResults = tableClient.QueryAsync<Order2>(filter: $"PartitionKey eq {orderId}");
            //await foreach (Order2 entity in queryResults)
            //{
            //    log.LogInformation($"save order 2 query {entity.PartitionKey}\t{entity.RowKey}\t{entity.Timestamp}\t{entity.Ordered}");
            //}
            //Make another funciton with a get Request and with the same route?
            //AsyncPageable<Order2> queryResultsLINQ =   tableClient.QueryAsync<Order2>(ent => ent.OrderId == orderId);
            var myTable = await tableClient.GetEntityAsync<Order2>("anotherTest", orderId);
            if(string.IsNullOrEmpty(myTable.Value.IsBilled.ToString()))
            {
                //myTable.Value.IsBilled = true;
                await tableClient.AddEntityAsync(tester);
            }
           

        }
    }
}

