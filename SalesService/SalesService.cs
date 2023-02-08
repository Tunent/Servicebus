using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Models;
using Newtonsoft.Json; 

namespace SalesService
{
    public class SalesService
    {
        [FunctionName("PlaceOrderV1")]
        [return: ServiceBus("billingqueue", Connection = "ServiceBusConnectionString")]
        public string Run([ServiceBusTrigger("orderQueue", 
            Connection = "ServiceBusConnectionString")]string queItem, 
            ILogger log)
        { 
            log.LogInformation($"C# PlaceOrder queue trigger function processed message: {queItem}");

            if(string.IsNullOrEmpty(queItem))
            {
                log.LogInformation("QueueItem is empty");
                return "";
            }
            var placeOrder = JsonConvert.DeserializeObject<PlaceOrder>(queItem);

            log.LogInformation($" placeOrder content : {placeOrder}");

            var orderPlacedEvent = new OrderPlaced
            {
                OrderId = Guid.NewGuid().ToString()
            };

            return JsonConvert.SerializeObject(orderPlacedEvent);  
        }
  
    }
}
