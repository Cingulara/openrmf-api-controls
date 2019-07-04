using System;
using Xunit;
using openstig_api_controls.Controllers;
using openstig_api_controls.Database;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace tests.Controllers
{

    public class ControlsControllerTests
    {
        private readonly Mock<ILogger<ControlsController>> _mockLogger;
        private readonly ControlsController _controlsController; 
        private readonly Mock<ControlsDBContext> _context;
        private readonly Mock<DbContextOptions<ControlsDBContext>> _contextOptions;

        public ControlsControllerTests() {
            _mockLogger = new Mock<ILogger<ControlsController>>();
            _contextOptions = new Mock<DbContextOptions<ControlsDBContext>>();
            _context = new Mock<ControlsDBContext>(_contextOptions);
            _controlsController = new ControlsController(_mockLogger.Object, _context.Object);
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
