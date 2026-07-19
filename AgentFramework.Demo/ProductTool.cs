using Intellectus.AIAgent.Framework;

namespace AgentFramework.Demo
{    
    public class ProductData
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }    

    public class ProductTool : ITool
    {
        public string Name => "ProductTool";
        public string Description => "Provides product information for a given product. Input: product name.";

        public Task<object> ExecuteAsync(params string[] input)
        {
            return Task.FromResult((object)new ProductData
            {
                ProductName = input[0],
                Description = "A high-quality product.",
                Price = 29.99m
            });
        }
    }
}