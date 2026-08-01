using System.Text.Json.Serialization;

namespace AIAgentFrameworkTests.Models
{
    public class ProductData
    {
        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
