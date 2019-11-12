using Xunit;
using openrmf_api_controls.Controllers;
using Moq;
using Microsoft.Extensions.Logging;

namespace tests.Controllers
{

    public class HealthControllerTests
    {
        private readonly Mock<ILogger<HealthController>> _mockLogger;
        private readonly HealthController _healthController; 

        public HealthControllerTests() {
            _mockLogger = new Mock<ILogger<HealthController>>();
            _healthController = new HealthController(_mockLogger.Object);
        }

        [Fact]
        public void Test_HealthControllerIsValid()
        {
            Assert.True(_healthController != null);
        }

        [Fact]
        public void Test_HealthControllerGetIsValid()
        {
            var result = _healthController.Get();
            Assert.True(_healthController != null);
            Assert.Equal(200, ((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).StatusCode); // returns a status code HTTP 200
        }
    }
}
