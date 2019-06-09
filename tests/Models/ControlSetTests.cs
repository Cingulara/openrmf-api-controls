using System;
using Xunit;
using openstig_api_controls.Models;
using Moq;

namespace tests
{
    public class ControlSetTests
    {
        [Fact]
        public void Test_NewControlSet()
        {
            ControlSet cs = new ControlSet();
            Assert.True(cs != null);
            Assert.True(cs.id != null);
        }
    }
}
