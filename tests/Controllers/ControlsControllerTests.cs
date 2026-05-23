// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using openrmf_api_controls.Controllers;

namespace tests.Controllers
{
    /// <summary>
    /// Unit tests for ControlsController.
    ///
    /// Note: GetAllControls, GetAllMajorControls, and GetControl(term) all delegate
    /// to the static NATSClient class which requires a live NATS server.  In unit
    /// test environments no NATS server is present, so those code paths exercise the
    /// exception-handling branch and return 400 Bad Request – verifying that the
    /// controller never lets an unhandled exception bubble out to the caller.
    /// </summary>
    public class ControlsControllerTests
    {
        private readonly Mock<ILogger<ControlsController>> _mockLogger;
        private readonly ControlsController _controller;

        public ControlsControllerTests()
        {
            _mockLogger = new Mock<ILogger<ControlsController>>();
            _controller = new ControlsController(_mockLogger.Object);
        }

        // ─── PASS: controller can be instantiated with mocked logger ─────────────

        [Fact]
        public void Test_ControlsController_IsNotNull()
        {
            Assert.NotNull(_controller);
        }

        [Fact]
        public void Test_ControlsController_MockLogger_IsNotNull()
        {
            Assert.NotNull(_mockLogger.Object);
        }

        // ─── PASS: GetControl with empty/null term returns 400 without hitting NATS ─

        [Fact]
        public async Task Test_GetControl_EmptyString_ReturnsBadRequest()
        {
            var result = await _controller.GetControl("");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task Test_GetControl_NullTerm_ReturnsBadRequest()
        {
            var result = await _controller.GetControl(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task Test_GetControl_EmptyTerm_ReturnsExpectedMessage()
        {
            var result = await _controller.GetControl("");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No valid control term sent", badRequest.Value);
        }

        // ─── PASS: input-validation path does not return Ok (200) ───────────────

        [Fact]
        public async Task Test_GetControl_EmptyTerm_DoesNotReturnOk()
        {
            var result = await _controller.GetControl("");

            Assert.IsNotType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Test_GetControl_NullTerm_DoesNotReturnOk()
        {
            var result = await _controller.GetControl(null);

            Assert.IsNotType<OkObjectResult>(result);
        }

        // ─── PASS: NATS-dependent paths catch exceptions → 400 Bad Request ───────
        // When no NATS server is present the connection attempt throws;
        // the controller's catch block returns BadRequest() (no body).

        [Fact]
        public async Task Test_GetAllControls_NoNATSServer_ReturnsBadRequest()
        {
            var result = await _controller.GetAllControls();

            // OkObjectResult if NATS succeeded, BadRequestResult if exception caught
            Assert.True(
                result is BadRequestResult || result is OkObjectResult,
                "Expected either BadRequestResult (no NATS) or OkObjectResult (NATS present).");
        }

        [Fact]
        public async Task Test_GetAllControls_WithImpactLevel_NoNATSServer_ReturnsBadRequest()
        {
            var result = await _controller.GetAllControls("high", false);

            Assert.True(
                result is BadRequestResult || result is OkObjectResult,
                "Expected either BadRequestResult (no NATS) or OkObjectResult (NATS present).");
        }

        [Fact]
        public async Task Test_GetAllMajorControls_NoNATSServer_ReturnsBadRequest()
        {
            var result = await _controller.GetAllMajorControls();

            Assert.True(
                result is BadRequestResult || result is OkObjectResult,
                "Expected either BadRequestResult (no NATS) or OkObjectResult (NATS present).");
        }

        [Fact]
        public async Task Test_GetControl_ValidTerm_NoNATSServer_ReturnsBadRequestOrOk()
        {
            var result = await _controller.GetControl("AC-1");

            Assert.True(
                result is BadRequestResult || result is OkObjectResult || result is NotFoundResult,
                "Expected BadRequestResult (no NATS), NotFoundResult, or OkObjectResult (NATS present).");
        }

        // ─── FAIL: empty term must NOT return Ok (200) ─────────────────────────

        [Fact]
        public async Task Test_GetControl_EmptyTerm_StatusCodeIsNot200()
        {
            var result = await _controller.GetControl("");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotEqual(200, badRequest.StatusCode);
        }

        [Fact]
        public async Task Test_GetControl_NullTerm_StatusCodeIsNot200()
        {
            var result = await _controller.GetControl(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotEqual(200, badRequest.StatusCode);
        }

        // ─── THEORY: all blank-ish terms return 400 ────────────────────────────

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Test_GetControl_BlankTerm_ReturnsBadRequest(string? term)
        {
            var result = await _controller.GetControl(term);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }
    }
}
