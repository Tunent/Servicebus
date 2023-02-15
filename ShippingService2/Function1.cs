using Azure;
using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace ShippingService2
{
    public class ShippingFunction
    {
        [FunctionName("ShippingFunction")]

        public static async Task Run(
            [ServiceBusTrigger("shippingqueue", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message,
            ILogger log,
           [Table("servicebustable", Connection = "BlobStorageConnectionString")] TableClient tableClient)
        {
            // 1. Determine which message is incomming
            log.LogInformation($"C# ServiceBus shipping queue trigger function processed message: {message}");
            var appliationPropertyType = message.ApplicationProperties.FirstOrDefault(x => x.Key == "Type");
            IOrderEvent orderEvent = null;
            switch (appliationPropertyType.Value)
            {
                case "OrderBilled":
                    orderEvent = JsonConvert.DeserializeObject<OrderBilled>(message.Body.ToString());
                    break;
                case "OrderPlaced":
                    orderEvent = JsonConvert.DeserializeObject<OrderPlaced>(message.Body.ToString());
                    break;
            }

            // 2. Get the table storage row.
            Pageable<Order> response = tableClient.Query<Order>(x => x.RowKey == orderEvent.OrderId);
            Order shipping = response.FirstOrDefault();
            bool shippingRecordExists = shipping != null;
            if (!shippingRecordExists)
            {
                shipping = new Order
                {
                    PartitionKey = "retaildemo",
                    RowKey = orderEvent.OrderId,
                    OrderId = orderEvent.OrderId
                };
            }

            if (orderEvent is OrderBilled)
            {
                shipping.IsBilled = true;
            }
            if (orderEvent is OrderPlaced)
            {
                shipping.IsOrdered = true;
                //tableClient.AddEntityAsync(Or)
            }

            // 3. Update or create the table storage row.
            if (!shippingRecordExists)
            {
                await tableClient.AddEntityAsync(shipping);

            }
            else
            {
                log.LogInformation("shipping waiting for IsOrdere or IsBilled");
                await tableClient.UpdateEntityAsync(shipping, ETag.All);

            }
            // 4. Determine if we are finished or not.
            if ((shipping.IsBilled && shipping.IsOrdered) == true)
            {
                log.LogInformation($"this order is shipped with orderId: {orderEvent.OrderId}");
            }
        }
    }
}
