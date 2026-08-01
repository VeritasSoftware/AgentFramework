using System.Text.Json.Serialization;

namespace AIAgentFrameworkTests.Models
{
    public class SalesData
    {
        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("totalSales")]
        public decimal TotalSales { get; set; }
        [JsonPropertyName("unitsSold")]
        public int UnitsSold { get; set; }
        [JsonPropertyName("year")]
        public int? Year { get; set; } = null; // Optional, default to null if not provided
    }
}
