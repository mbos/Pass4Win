namespace Pass4Win.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// The pwgen tests.
    /// </summary>
    [TestFixture()]
    public class PwgenTests
    {
        /// <summary>
        /// Tests generation of small and very large length
        /// </summary>
        [Test()]
        public void PwgenTest()
        {
            string result = Pwgen.Generate(10);
            Assert.AreEqual(result.Length, 10);

            string result2 = Pwgen.Generate(80);
            Assert.AreEqual(result2.Length, 80);
        }
    }
}