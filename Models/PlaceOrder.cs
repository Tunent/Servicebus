namespace Models
{
    public class PlaceOrder : IOrderEvent
    {
        public string OrderId { get; set; }
    }
}
