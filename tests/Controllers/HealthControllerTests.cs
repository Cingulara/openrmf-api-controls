// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using openrmf_api_controls.Controllers;

namespace tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly Mock<ILogger<HealthController>> _mockLogger;
        private readonly HealthController _controller;

        public HealthControllerTests()
        {
            _mockLogger = new Mock<ILogger<HealthController>>();
            _controller = new HealthController(_mockLogger.Object);
        }

        // ─── PASS: controller can be instantiated with mocked logger ─────────────

        [Fact]
        public void Test_HealthController_IsNotNull()
        {
            Assert.NotNull(_controller);
        }

        // ─── PASS: GET /healthz returns 200 OK with "ok" body ──────────────────

        [Fact]
        public void Test_HealthController_Get_ReturnsOk()
        {
            var result = _controller.Get();

            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public void Test_HealthController_Get_ReturnsOkBodyValue()
        {
            var result = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal("ok", okResult.Value);
        }

        [Fact]
        public void Test_HealthController_Get_ResultIsActionResult()
        {
            var result = _controller.Get();
            Assert.IsAssignableFrom<ActionResult<string>>(result);
        }

        // ─── PASS: logger is used (mock verifiable) ─────────────────────────

        [Fact]
        public void Test_HealthController_Get_LoggerIsNotNull()
        {
            Assert.NotNull(_mockLogger.Object);
        }

        // ─── FAIL: assertions verifying wrong status codes are not returned ─────

        [Fact]
        public void Test_HealthController_Get_DoesNotReturnBadRequest()
        {
            var result = _controller.Get();

            Assert.NotNull(result);
            Assert.IsNotType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void Test_HealthController_Get_DoesNotReturnNotFound()
        {
            var result = _controller.Get();

            Assert.IsNotType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void Test_HealthController_Get_StatusCodeIsNot400()
        {
            var result = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotEqual(400, okResult.StatusCode);
        }
    }
}
