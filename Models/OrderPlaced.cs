namespace Models
{
    public class OrderPlaced : IOrderEvent
    {
        public string? OrderId { get; set; }
    }
}
