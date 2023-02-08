using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ShippingService
{
    public class ShippingService
    {
        [FunctionName("Function1")]
        public void Run([ServiceBusTrigger("shippingQueue", Connection = "")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
