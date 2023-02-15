using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json;

namespace SalesService
{
    public class SalesService
    {
        [FunctionName("PlaceOrderV1")]
        [return: ServiceBus("trevortopic", Connection = "ServiceBusConnectionString")]
        public static ServiceBusMessage Run(
            [ServiceBusTrigger(queueName:"orderqueue",
            Connection = "ServiceBusConnectionString")]string queItem,
            ILogger log)
        {
            log.LogInformation($"C# PlaceOrder queue trigger function processed message: {queItem}");

            PlaceOrder placeOrder = JsonConvert.DeserializeObject<PlaceOrder>(queItem);

            log.LogInformation($" placeOrder content : {placeOrder}");

            var orderPlaced = new OrderPlaced
            {
                OrderId = placeOrder.OrderId
            };

            ServiceBusMessage message = new ServiceBusMessage(JsonConvert.SerializeObject(orderPlaced));
            message.ApplicationProperties.Add("Type", "OrderPlaced");

            return message;
        }

    }
}
