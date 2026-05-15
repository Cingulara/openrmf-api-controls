// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license.

using System;
using Xunit;
using openrmf_api_controls.Classes;

namespace tests.Classes
{
    /// <summary>
    /// Tests for the NATSClient static class.
    ///
    /// NATSClient communicates with a live NATS server via the NATSSERVERURL
    /// environment variable.  In a unit-test environment no server is present, so
    /// all tests verify that:
    ///   1. The static class is accessible (not null by type).
    ///   2. Every method throws predictably when the server is unreachable,
    ///      confirming that callers (controllers) must handle exceptions.
    ///
    /// Integration tests against a real NATS server are out of scope here.
    /// </summary>
    public class NATSClientTests
    {
        // ─── PASS: type-level sanity checks ──────────────────────────────────────

        [Fact]
        public void Test_NATSClient_TypeExists()
        {
            var type = typeof(NATSClient);
            Assert.NotNull(type);
        }

        [Fact]
        public void Test_NATSClient_IsStaticClass()
        {
            var type = typeof(NATSClient);
            Assert.True(type.IsAbstract && type.IsSealed,
                "NATSClient should be a static class (abstract + sealed).");
        }

        [Fact]
        public void Test_NATSClient_HasGetControlRecordsMethod()
        {
            var method = typeof(NATSClient).GetMethod("GetControlRecords");
            Assert.NotNull(method);
        }

        [Fact]
        public void Test_NATSClient_HasGetControlRecordMethod()
        {
            var method = typeof(NATSClient).GetMethod("GetControlRecord");
            Assert.NotNull(method);
        }

        // ─── PASS: GetControlRecords method signature is correct ─────────────────

        [Fact]
        public void Test_NATSClient_GetControlRecords_ReturnsListType()
        {
            var method = typeof(NATSClient).GetMethod("GetControlRecords");
            Assert.NotNull(method);
            Assert.Equal(
                typeof(System.Collections.Generic.List<openrmf_api_controls.Models.ControlSet>),
                method!.ReturnType);
        }

        [Fact]
        public void Test_NATSClient_GetControlRecord_ReturnsControlSetType()
        {
            var method = typeof(NATSClient).GetMethod("GetControlRecord");
            Assert.NotNull(method);
            Assert.Equal(
                typeof(openrmf_api_controls.Models.ControlSet),
                method!.ReturnType);
        }

        // ─── FAIL: calling methods without a NATS server throws an exception ──────
        // These tests confirm that the static methods bubble up an exception when
        // the server is not reachable – the controller layer is responsible for
        // catching that exception and returning the appropriate HTTP status code.

        [Fact]
        public void Test_GetControlRecords_NoServer_ThrowsException()
        {
            // Ensure no valid NATS URL is set so the connection always fails
            Environment.SetEnvironmentVariable("NATSSERVERURL", "nats://localhost:14222");

            Assert.ThrowsAny<Exception>(() =>
                NATSClient.GetControlRecords("low", false));
        }

        [Fact]
        public void Test_GetControlRecords_WithPii_NoServer_ThrowsException()
        {
            Environment.SetEnvironmentVariable("NATSSERVERURL", "nats://localhost:14222");

            Assert.ThrowsAny<Exception>(() =>
                NATSClient.GetControlRecords("high", true));
        }

        [Fact]
        public void Test_GetControlRecord_NoServer_ThrowsException()
        {
            Environment.SetEnvironmentVariable("NATSSERVERURL", "nats://localhost:14222");

            Assert.ThrowsAny<Exception>(() =>
                NATSClient.GetControlRecord("AC-1"));
        }

        [Fact]
        public void Test_GetControlRecord_EmptyTerm_NoServer_ThrowsException()
        {
            Environment.SetEnvironmentVariable("NATSSERVERURL", "nats://localhost:14222");

            Assert.ThrowsAny<Exception>(() =>
                NATSClient.GetControlRecord(string.Empty));
        }

        // ─── THEORY: any impact level raises an exception without a server ────────

        [Theory]
        [InlineData("low",      false)]
        [InlineData("moderate", false)]
        [InlineData("high",     false)]
        [InlineData("high",     true)]
        public void Test_GetControlRecords_AnyImpactLevel_NoServer_ThrowsException(
            string impactLevel, bool pii)
        {
            Environment.SetEnvironmentVariable("NATSSERVERURL", "nats://localhost:14222");

            Assert.ThrowsAny<Exception>(() =>
                NATSClient.GetControlRecords(impactLevel, pii));
        }
    }
}
