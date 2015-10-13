using NUnit.Framework;

namespace Pass4Win.Tests
{
    [TestFixture()]
    public class PwgenTests
    {
        [Test()]
        public void PwgenTest()
        {
            string result = Pass4Win.Pwgen.Generate(10);
            Assert.AreEqual(result.Length, 10);

            string result2 = Pass4Win.Pwgen.Generate(80);
            Assert.AreEqual(result2.Length, 80);
        }
    }
}