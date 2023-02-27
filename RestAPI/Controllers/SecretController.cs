using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretController : ControllerBase
    {
        //private readonly IConfiguration _config;

        //public SecretController(IConfiguration config)
        //{
        //    _config = config;
        //}

        //[HttpGet]
        //public IActionResult GetKey()
        //{
        //    //var number = _config.GetValue<string>("ServiceBusConnectionTwo");
        //    //return Content($"{number}");
        //    //var connectionString = _config.GetConnectionString("ServiceBusConnectionTwo");
        //    //return new JsonResult(connectionString);
        //    //return Ok(connectionString);
        //}
    }
}
