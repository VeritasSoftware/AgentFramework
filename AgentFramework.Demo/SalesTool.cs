using Intellectus.AIAgent.Framework;

namespace AgentFramework.Demo
{
    public class SalesData
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int UnitsSold { get; set; }
        public int? Year { get; set; } = null; // Optional, default to null if not provided
    }

    public class SalesTool : ITool
    {
        public string Name => "SalesTool";
        public string Description => "Provides sales data for a given product. Input: product name, year.";

        public Task<object> ExecuteAsync(params string[] input)
        {
            var productName = input[0].Trim();
            var year =
                input.Length > 1
                ?
                int.Parse(input[1].Trim())
                :
                0;

            return Task.FromResult((object)
            (year == 0
            ?
            //Total sales and units sold for the product without year
            new SalesData
            {
                ProductName = productName,
                TotalSales = 10000.50m,
                UnitsSold = 2000
            }
            :
            //Total sales and units sold for the product for the specified year
            new SalesData
            {
                ProductName = productName,
                TotalSales = 500.50m,
                UnitsSold = 50,
                Year = year
            }));
        }
    }
}
