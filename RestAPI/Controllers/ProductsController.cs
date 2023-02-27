using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

namespace RestAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private static readonly Product[] Products = new[]
        {
            new Product() { Name = "Book", Price = 9.99M, Manufacturer = "O'Reilly" },
            new Product() { Name = "Car", Price = 45000, Manufacturer = "Tesla" },
            new Product() { Name = "Starship", Price = 9999999999, Manufacturer = "SpaceX" },
        };

        // GET: api/<ProductController>
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return Products;
        }

        // POST: api/<ProductController>/order
        [HttpPost("order")]
        public async Task<IActionResult> OrderProductAsync(string productName)
        {
            var product = Products.FirstOrDefault(x => x.Name == productName);
            if (product is null)
            {
                return new NotFoundObjectResult($"Product {productName} not found in catalogue.");
            }

            // Create serialised ServiceBusMessage object
            OrderInfo orderInfo = new OrderInfo
            {
                OrderId = Guid.NewGuid().ToString(),
                Product = product,
                Buyer = "He-Man"
            };
            var serialisedOrder = JsonConvert.SerializeObject(orderInfo);
            ServiceBusMessage serviceBusMessage = new ServiceBusMessage(serialisedOrder);
            serviceBusMessage.ApplicationProperties.Add("Type", "PlaceOrder");

            // Setup ServiceBus communication
            try
            {
                var connectionString = _configuration.GetConnectionString("ServiceBusConnection2");
                ServiceBusClient client = new ServiceBusClient(connectionString);
                // retrieve App Service connection string
                ServiceBusSender sender = client.CreateSender("trevortopic");

                // Create a batch and send message
                using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
                messageBatch.TryAddMessage(serviceBusMessage);

                await sender.SendMessagesAsync(messageBatch);

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}

