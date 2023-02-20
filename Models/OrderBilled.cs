namespace Models
{
    public class OrderBilled : IOrderEvent
    {
        public string? OrderId { get; set; }
    }
}
