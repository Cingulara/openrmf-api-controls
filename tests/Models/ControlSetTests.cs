using Xunit;
using openrmf_api_controls.Models;

namespace tests.Models
{
    public class ControlSetTests
    {
        [Fact]
        public void Test_NewControlSetIsValid()
        {
            ControlSet cs = new ControlSet();
            Assert.True(cs != null);
            Assert.True(cs.id != null);
            Assert.False(cs.lowimpact);
            Assert.False(cs.moderateimpact);
            Assert.False(cs.highimpact);
        }

        [Fact]
        public void Test_ControlSetWidhDataIsValid()
        {
            ControlSet cs = new ControlSet();
            cs.family = "AC";
            cs.number = "AC-1";
            cs.title = "My AC title";
            cs.priority = "P1";
            cs.supplementalGuidance = "My supplemental guidance.";
            cs.subControlDescription = "My subcontrol";
            cs.subControlNumber = "AC-1.1";
            Assert.True(cs != null);
            Assert.True(cs.id != null);
            Assert.False(cs.lowimpact);
            Assert.False(cs.moderateimpact);
            Assert.False(cs.highimpact);
        }
    }
}
