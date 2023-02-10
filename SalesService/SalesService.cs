using System;
using System.Threading.Tasks;
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
        public  static ServiceBusMessage Run(
            [ServiceBusTrigger(queueName:"orderqueue",
            Connection = "ServiceBusConnectionString")]string queItem, 
            ILogger log)
        { 
            log.LogInformation($"C# PlaceOrder queue trigger function processed message: {queItem}");

            PlaceOrder placeOrder = JsonConvert.DeserializeObject<PlaceOrder>(queItem);

            log.LogInformation($" placeOrder content : {placeOrder}");

            var orderPlaced = new OrderPlaced
            {
                OrderId = placeOrder.Id
            };

            ////return JsonConvert.SerializeObject(orderPlacedEvent);
            //var serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(orderPlacedEvent));
            //serviceBusMessage.ApplicationProperties.Add("Type", "OrderPlaced");



            //return serviceBusMessage;
            ServiceBusMessage message = new ServiceBusMessage(JsonConvert.SerializeObject(orderPlaced));
            message.ApplicationProperties.Add("Type", "OrderPlaced");
            //message.MessageId = Guid.NewGuid().ToString();
            //message.Body = new BinaryData(JsonConvert.SerializeObject(orderPlaced));
            return message;
        }
  
    }
}
