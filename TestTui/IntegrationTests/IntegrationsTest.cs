using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace IntegrationTests
{
    public class IntegrationsTest
    {
        [Fact]
        public void AddProduct_Correct_Input_Save_In_Context()
        {
            using (TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>()))
            {
                using (HttpClient client = server.CreateClient())
                {
                    string productJson = "{\"Code\":\"TEST\",\"EndValidatyDate\":\"2021-12-31T00:00:00.000Z\",\"Name\":\"TEST\",\"StartValidityDate\":\"2021-01-01T00:00:00.000Z\"}";
                    HttpContent content = new StringContent(productJson, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync("Product", content).Result;
                    response.EnsureSuccessStatusCode();

                    string contentResponse = response.Content.ReadAsStringAsync().Result;
                    ProductModel product = JsonConvert.DeserializeObject<ProductModel>(contentResponse);

                    Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
                    Assert.NotNull(product);
                }
            }
        }

        [Fact]
        public void GetAllProducts_Return_Filled_Products_List()
        {
            using (TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>()))
            {
                using (HttpClient client = server.CreateClient())
                {
                    HttpResponseMessage response = client.GetAsync("Product").Result;
                    response.EnsureSuccessStatusCode();

                    string contentResponse = response.Content.ReadAsStringAsync().Result;
                    IEnumerable<ProductModel> products = JsonConvert.DeserializeObject<IEnumerable<ProductModel>>(contentResponse);

                    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
                    Assert.NotNull(products);
                }
            }
        }
    }
}
