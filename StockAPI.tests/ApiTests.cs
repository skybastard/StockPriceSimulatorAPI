using Microsoft.AspNetCore.Mvc.Testing;
using StockPriceSimulatorAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StockAPI.tests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetPrices_ReturnsSuccessAndContent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/prices");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrWhiteSpace(json));
        }

        [Fact]
        public async Task GetPriceBySymbol_ReturnsStockWithHistory()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/prices/AAPL");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();

            // Deserialize to check structure
            var doc = JsonDocument.Parse(json);

            Assert.True(doc.RootElement.TryGetProperty("name", out _));
            Assert.True(doc.RootElement.TryGetProperty("currentPrice", out _));
            Assert.True(doc.RootElement.TryGetProperty("history", out _));
        }
    }
}
