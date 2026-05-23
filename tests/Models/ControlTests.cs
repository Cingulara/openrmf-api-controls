// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license.

using System;
using System.Collections.Generic;
using Xunit;
using openrmf_api_controls.Models;

namespace tests.Models
{
    public class ControlTests
    {
        // ─── PASS: default constructor initializes properly ───────────────────────

        [Fact]
        public void Test_NewControl_IsNotNull()
        {
            var c = new Control();
            Assert.NotNull(c);
        }

        [Fact]
        public void Test_NewControl_IdIsNotEmpty()
        {
            var c = new Control();
            Assert.NotEqual(Guid.Empty, c.id);
        }

        [Fact]
        public void Test_NewControl_ChildControlsInitializedEmpty()
        {
            var c = new Control();
            Assert.NotNull(c.childControls);
            Assert.Empty(c.childControls);
        }

        [Fact]
        public void Test_NewControl_ImpactFlagsDefaultFalse()
        {
            var c = new Control();
            Assert.False(c.lowimpact);
            Assert.False(c.moderateimpact);
            Assert.False(c.highimpact);
        }

        // ─── PASS: properties can be set and read back correctly ──────────────────

        [Fact]
        public void Test_Control_WithData_PropertiesAreSet()
        {
            var c = new Control
            {
                family = "AC",
                number = "AC-1",
                title = "Access Control Policy and Procedures",
                priority = "P1",
                supplementalGuidance = "Supplemental guidance text.",
                lowimpact = true,
                moderateimpact = true,
                highimpact = true
            };

            Assert.Equal("AC", c.family);
            Assert.Equal("AC-1", c.number);
            Assert.Equal("Access Control Policy and Procedures", c.title);
            Assert.Equal("P1", c.priority);
            Assert.Equal("Supplemental guidance text.", c.supplementalGuidance);
            Assert.True(c.lowimpact);
            Assert.True(c.moderateimpact);
            Assert.True(c.highimpact);
        }

        [Fact]
        public void Test_Control_AddChildControl_CountIncreases()
        {
            var cc = new ChildControl
            {
                description = "Supplementary statement.",
                number = "AC-1 (a)"
            };

            var c = new Control();
            c.childControls.Add(cc);

            Assert.Single(c.childControls);
            Assert.Equal("Supplementary statement.", c.childControls[0].description);
            Assert.Equal("AC-1 (a)", c.childControls[0].number);
        }

        [Fact]
        public void Test_Control_MultipleChildControls_AllAdded()
        {
            var c = new Control();
            c.childControls.Add(new ChildControl { description = "Part a.", number = "AC-1a" });
            c.childControls.Add(new ChildControl { description = "Part b.", number = "AC-1b" });

            Assert.Equal(2, c.childControls.Count);
        }

        [Fact]
        public void Test_Control_EachInstanceHasUniqueId()
        {
            var c1 = new Control();
            var c2 = new Control();
            Assert.NotEqual(c1.id, c2.id);
        }

        // ─── PASS: ChildControl defaults ─────────────────────────────────────────

        [Fact]
        public void Test_ChildControl_IsNotNull()
        {
            var cc = new ChildControl();
            Assert.NotNull(cc);
        }

        [Fact]
        public void Test_ChildControl_IdIsNotEmpty()
        {
            var cc = new ChildControl();
            Assert.NotEqual(Guid.Empty, cc.id);
        }

        [Fact]
        public void Test_ChildControl_EachInstanceHasUniqueId()
        {
            var cc1 = new ChildControl();
            var cc2 = new ChildControl();
            Assert.NotEqual(cc1.id, cc2.id);
        }

        [Fact]
        public void Test_ChildControl_WithData_PropertiesAreSet()
        {
            var cc = new ChildControl
            {
                description = "Detailed statement.",
                number = "AC-1.1"
            };
            Assert.Equal("Detailed statement.", cc.description);
            Assert.Equal("AC-1.1", cc.number);
        }

        // ─── FAIL: assertions that verify incorrect data is rejected ───────────────

        [Fact]
        public void Test_Control_EmptyChildControls_CountIsNotOne()
        {
            var c = new Control();
            Assert.NotEqual(1, c.childControls.Count);
        }

        [Fact]
        public void Test_Control_DefaultImpact_IsNotHigh()
        {
            var c = new Control();
            Assert.False(c.highimpact);
        }

        [Fact]
        public void Test_Control_IdIsNotEqualToDefault()
        {
            var c = new Control();
            Assert.NotEqual(Guid.Empty, c.id);
        }

        // ─── THEORY: impact level combinations ────────────────────────────────────

        [Theory]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(true, true, true)]
        public void Test_Control_ImpactFlags_CanBeSetIndependently(
            bool low, bool moderate, bool high)
        {
            var c = new Control
            {
                lowimpact = low,
                moderateimpact = moderate,
                highimpact = high
            };
            Assert.Equal(low, c.lowimpact);
            Assert.Equal(moderate, c.moderateimpact);
            Assert.Equal(high, c.highimpact);
        }
    }
}
