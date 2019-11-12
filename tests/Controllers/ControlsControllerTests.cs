using Xunit;
using openrmf_api_controls.Controllers;
using Moq;
using Microsoft.Extensions.Logging;

namespace tests.Controllers
{

    public class ControlsControllerTests
    {
        private readonly Mock<ILogger<ControlsController>> _mockLogger;
        private readonly ControlsController _controlsController; 

        public ControlsControllerTests() {
            _mockLogger = new Mock<ILogger<ControlsController>>();
            _controlsController = new ControlsController(_mockLogger.Object);
        }

        [Fact]
        public void Test_ControlsControllerIsValid()
        {
            Assert.True(_controlsController != null);
        }

        [Fact]
        public void Test_ControlsControllerGetAllControllersNoFilterIsValid()
        {
            var result = _controlsController.GetAllControls();
            Assert.True(_controlsController != null);
            Assert.Equal(200, ((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).StatusCode); // returns a status code HTTP 200
        }

        [Fact]
        public void Test_ControlsControllerGetAllControllersFilterIsValid()
        {
            var result = _controlsController.GetAllControls("AC-1");
            Assert.True(_controlsController != null);
            Assert.Equal(200, ((Microsoft.AspNetCore.Mvc.ObjectResult)result.Result).StatusCode); // returns a status code HTTP 200
        }

    }
}
