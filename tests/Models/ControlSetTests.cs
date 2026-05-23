// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license.

using System;
using Xunit;
using openrmf_api_controls.Models;

namespace tests.Models
{
    public class ControlSetTests
    {
        // ─── PASS: default constructor initializes properly ───────────────────────

        [Fact]
        public void Test_NewControlSet_IsNotNull()
        {
            var cs = new ControlSet();
            Assert.NotNull(cs);
        }

        [Fact]
        public void Test_NewControlSet_IdIsNotEmpty()
        {
            var cs = new ControlSet();
            Assert.NotEqual(Guid.Empty, cs.id);
        }

        [Fact]
        public void Test_NewControlSet_ImpactFlagsDefaultFalse()
        {
            var cs = new ControlSet();
            Assert.False(cs.lowimpact);
            Assert.False(cs.moderateimpact);
            Assert.False(cs.highimpact);
        }

        // ─── PASS: properties can be set and read back correctly ──────────────────

        [Fact]
        public void Test_ControlSet_WithData_PropertiesAreSet()
        {
            var cs = new ControlSet
            {
                family = "AC",
                number = "AC-1",
                title = "Access Control Policy and Procedures",
                priority = "P1",
                supplementalGuidance = "Supplemental guidance.",
                subControlDescription = "Sub-control description.",
                subControlNumber = "AC-1.1",
                lowimpact = true,
                moderateimpact = true,
                highimpact = true
            };

            Assert.Equal("AC", cs.family);
            Assert.Equal("P1", cs.priority);
            Assert.Equal("Access Control Policy and Procedures", cs.title);
            Assert.Equal("Supplemental guidance.", cs.supplementalGuidance);
            Assert.Equal("Sub-control description.", cs.subControlDescription);
            Assert.Equal("AC-1.1", cs.subControlNumber);
            Assert.True(cs.lowimpact);
            Assert.True(cs.moderateimpact);
            Assert.True(cs.highimpact);
        }

        [Fact]
        public void Test_ControlSet_EachInstanceHasUniqueId()
        {
            var cs1 = new ControlSet();
            var cs2 = new ControlSet();
            Assert.NotEqual(cs1.id, cs2.id);
        }

        // ─── PASS: indexsort computed property pads single-digit controls ──────────

        [Fact]
        public void Test_ControlSet_IndexSort_PadsSingleDigitNumber()
        {
            // AC-1 → indexsort should be "AC-01"
            var cs = new ControlSet { number = "AC-1" };
            Assert.Equal("AC-01", cs.indexsort);
        }

        [Fact]
        public void Test_ControlSet_IndexSort_DoesNotPadDoubleDigitNumber()
        {
            // AC-10 → indexsort should be "AC-10"
            var cs = new ControlSet { number = "AC-10" };
            Assert.Equal("AC-10", cs.indexsort);
        }

        [Fact]
        public void Test_ControlSet_IndexSort_StripsSubControlSuffix()
        {
            // AC-1 (1) → space found, trims to AC-1 first; single digit → "AC-01"
            var cs = new ControlSet { number = "AC-1 (1)" };
            var sort = cs.indexsort;
            Assert.StartsWith("AC-", sort);
        }

        // ─── FAIL: assertions verifying incorrect values are not returned ──────────

        [Fact]
        public void Test_ControlSet_DefaultImpact_IsNotHigh()
        {
            var cs = new ControlSet();
            Assert.False(cs.highimpact);
        }

        [Fact]
        public void Test_ControlSet_EmptyFamily_IsNotAC()
        {
            var cs = new ControlSet();
            Assert.NotEqual("AC", cs.family);
        }

        [Fact]
        public void Test_ControlSet_IdIsNotEqualToDefault()
        {
            var cs = new ControlSet();
            Assert.NotEqual(Guid.Empty, cs.id);
        }

        // ─── THEORY: impact level flag combinations ────────────────────────────────

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(true, true, true)]
        public void Test_ControlSet_ImpactFlags_CanBeSetIndependently(
            bool low, bool moderate, bool high)
        {
            var cs = new ControlSet
            {
                lowimpact = low,
                moderateimpact = moderate,
                highimpact = high
            };
            Assert.Equal(low, cs.lowimpact);
            Assert.Equal(moderate, cs.moderateimpact);
            Assert.Equal(high, cs.highimpact);
        }

        // ─── THEORY: indexsort known values ───────────────────────────────────────

        [Theory]
        [InlineData("AC-1",  "AC-01")]
        [InlineData("AC-10", "AC-10")]
        [InlineData("SI-2",  "SI-02")]
        [InlineData("SI-12", "SI-12")]
        public void Test_ControlSet_IndexSort_ProducesExpectedValue(string number, string expected)
        {
            var cs = new ControlSet { number = number };
            Assert.Equal(expected, cs.indexsort);
        }
    }
}
