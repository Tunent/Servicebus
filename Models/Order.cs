using Azure;
using Azure.Data.Tables;

namespace Models
{
    public class Order : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string OrderId { get; set; }
        public bool IsOrdered { get; set; }
        public bool IsBilled { get; set; }
    }
}
