namespace Pass4Win.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// The pwgen tests.
    /// </summary>
    [TestFixture()]
    public class PwgenTests
    {
      
        [Test()]
        public void GeneratePasswordWithTenCharactersTest()
        {
            string result = Pwgen.Generate(10);
            Assert.AreEqual(result.Length, 10);
        }

        [Test()]
        public void GeneratePasswordWithEightyCharactersTest()
        {
            string result2 = Pwgen.Generate(80);
            Assert.AreEqual(result2.Length, 80);
        }
    }
}