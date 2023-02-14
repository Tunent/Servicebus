using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ShippingService2
{
        public class ShippingFunction
        {
            [FunctionName("ShippingFunction")]
            //[return: ServiceBus("Orders", Connection = "ServiceBusConnectionString")]
            public static async Task Run(
                [ServiceBusTrigger("shippingqueue", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage message,
                ILogger log)
            {
                log.LogInformation($"C# ServiceBus shipping queue trigger function processed message: {message}");

                foreach (var prop in message.ApplicationProperties)
                {
                    log.LogInformation($": {prop.Key}: {prop.Value}");
                }



            }
        }
}
