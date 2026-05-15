// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license.

using Xunit;
using openrmf_api_controls.Models;

namespace tests.Models
{
    public class FilterTests
    {
        // ─── PASS: default constructor initializes properly ───────────────────────

        [Fact]
        public void Test_NewFilter_IsNotNull()
        {
            var f = new Filter();
            Assert.NotNull(f);
        }

        [Fact]
        public void Test_NewFilter_DefaultImpactLevelIsLow()
        {
            var f = new Filter();
            Assert.Equal("low", f.impactLevel);
        }

        [Fact]
        public void Test_NewFilter_PiiDefaultsFalse()
        {
            var f = new Filter();
            Assert.False(f.pii);
        }

        // ─── PASS: properties can be set and read back correctly ──────────────────

        [Fact]
        public void Test_Filter_SetImpactLevel_Moderate()
        {
            var f = new Filter { impactLevel = "moderate" };
            Assert.Equal("moderate", f.impactLevel);
        }

        [Fact]
        public void Test_Filter_SetImpactLevel_High()
        {
            var f = new Filter { impactLevel = "high" };
            Assert.Equal("high", f.impactLevel);
        }

        [Fact]
        public void Test_Filter_SetPii_True()
        {
            var f = new Filter { pii = true };
            Assert.True(f.pii);
        }

        [Fact]
        public void Test_Filter_SetBothProperties()
        {
            var f = new Filter
            {
                impactLevel = "high",
                pii = true
            };
            Assert.Equal("high", f.impactLevel);
            Assert.True(f.pii);
        }

        // ─── FAIL: assertions that verify incorrect values are rejected ───────────

        [Fact]
        public void Test_Filter_DefaultImpactLevel_IsNotHigh()
        {
            var f = new Filter();
            Assert.NotEqual("high", f.impactLevel);
        }

        [Fact]
        public void Test_Filter_DefaultImpactLevel_IsNotModerate()
        {
            var f = new Filter();
            Assert.NotEqual("moderate", f.impactLevel);
        }

        [Fact]
        public void Test_Filter_DefaultPii_IsNotTrue()
        {
            var f = new Filter();
            Assert.False(f.pii);
        }

        // ─── THEORY: valid impact level strings ───────────────────────────────────

        [Theory]
        [InlineData("low")]
        [InlineData("moderate")]
        [InlineData("high")]
        public void Test_Filter_ImpactLevel_CanBeSetToValidValue(string level)
        {
            var f = new Filter { impactLevel = level };
            Assert.Equal(level, f.impactLevel);
        }
    }
}
