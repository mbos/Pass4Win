using System;

using NUnit.Framework;

namespace Pass4Win
{
    [TestFixture]
    public class Pass4Win_PWGenUnitTests
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            // nothing to setup
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            // nothing to destroy
        }

        [Test]
        public void TestHowManyChars()
        {
            string result = pwgen.Generate(10);
            Assert.AreEqual(result.Length, 10);

            string result2 = pwgen.Generate(80);
            Assert.AreEqual(result2.Length, 80);
        }
    }
}
