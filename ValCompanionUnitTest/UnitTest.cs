using System.Diagnostics;
using System.Threading;
using ValRestServer;

namespace ValCompanionUnitTest
{
    public class Tests
    {
    
        [SetUp]
        public void Setup()
        {
            
        }


        [Test]
        public void CheckIfGameIsRunning()
        {
            Process[] process = Process.GetProcessesByName("VALORANT");

            Assert.True(process.Length > 0);
        }
    }
}