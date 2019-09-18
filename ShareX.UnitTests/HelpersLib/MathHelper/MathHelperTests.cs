using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ShareX.HelpersLib;

namespace ShareX.UnitTests.HelpersLib.MathHelper
{
    [TestFixture]
    public  class MathHelperTests
    {
        [Test]
        public void Min_WhenCalled_ShouldReturnTheMinimum()
        {
            var result = MathHelpers.Min(1, 2);
            Assert.That(result,Is.EqualTo(1));
        }
        [Test]
        public void Max_WhenCalled_ShouldReturnTheMaximum()
        {
            var result = MathHelpers.Max(1, 2);
            Assert.That(result,Is.EqualTo(2));
        }

        [Test]
        public void IsBetween_WhenFirstIsBetweenTwoOthers_ShouldReturnTrue()
        {
            var result = MathHelpers.IsBetween(0, -1, 1);
            Assert.That(result,Is.True);
        }

        [Test]
        public void IsBetween_WhenFirstIsNotBetweenTwoOthers_ShouldReturnFalse()
        {
            var result = MathHelpers.IsBetween(0, 1, 2);
            Assert.That(result,Is.False);
        }

        [Test]
        [TestCase(0,0,1)]
        [TestCase(1,0,1)]
        public void IsBetween_WhenFirstIsEqualTwoOneOfTwo_ShouldReturnTrue(int between,int first,int second)
        {
            var result = MathHelpers.IsBetween(between,first,second);
            Assert.That(result,Is.True);
        }
    }
}
