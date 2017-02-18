using System.Threading.Tasks;
using FluentAssertions;
using ContactsCore.Model.ViewModels;
using Xunit;

namespace ContactsCore.Api.Integration
{
    public class HealthcheckShould : BaseTest
    {        
        private const string HealthcheckPath = "healthcheck";        
        
        [Fact]
        public async Task Get()
        {
            using (var client = GetClient())
            {
                // get
                var response = await client.GetAsync(HealthcheckPath);


                var responseModel = await DeserializeResponse<HealthcheckViewModel>(response);
                responseModel.ApiVersion.Should().NotBeNullOrWhiteSpace();
                responseModel.DbStatus.Should().Be("OK");
            }
        }
    }
}