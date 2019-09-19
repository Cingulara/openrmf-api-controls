using Xunit;
using openstig_api_controls.Models;

namespace tests.Models
{
    public class ControlTests
    {
        [Fact]
        public void Test_NewControlIsValid()
        {
            Control cs = new Control();
            Assert.True(cs != null);
            Assert.True(cs.id != null);
            Assert.True(cs.childControls.Count == 0);
            Assert.False(cs.lowimpact);
            Assert.False(cs.moderateimpact);
            Assert.False(cs.highimpact);
        }
    
        [Fact]
        public void Test_ControlWithDataIsValid()
        {
            ChildControl cc = new ChildControl();
            cc.description = "My Child Description";
            cc.number = "AC-1.1";

            Control cs = new Control();
            cs.family = "AC";
            cs.number = "AC-1";
            cs.title = "My Title";
            cs.priority = "P1";
            cs.childControls.Add(cc);
            cs.supplementalGuidance = "My supplemental guidance.";
            Assert.True(cs != null);
            Assert.True(cs.id != null);
            Assert.True(cs.childControls.Count == 1);
            Assert.True(cs.childControls[0].id != null);
            Assert.False(cs.lowimpact);
            Assert.False(cs.moderateimpact);
            Assert.False(cs.highimpact);
        }
    }
}
