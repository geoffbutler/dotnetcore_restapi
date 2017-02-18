using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContactsCore.Api.Integration
{
    public abstract class BaseTest
    {
        protected static readonly Uri ApiRootUri;

        static BaseTest()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            ApiRootUri = new Uri(config["ApiRoot"]);
        }

        protected static class ContentType
        {
            public const string ApplicationJson = "application/json";
        }
        
        protected static HttpClient GetClient()
        {
            var client = new HttpClient {BaseAddress = ApiRootUri};
            return client;
        }

        protected static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
           where T : new()
        {
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotBeNull();

            var responseModel = JsonConvert.DeserializeObject<T>(responseContent);
            responseModel.Should().NotBeNull();

            return responseModel;
        }
    }
}
