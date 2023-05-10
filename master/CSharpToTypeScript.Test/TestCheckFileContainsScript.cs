using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    [TestClass]
    public class TestCheckFileContainsScript : IDisposable
    {
        private bool disposedValue;

        public TestContext TestContext { get; set; } = null!;

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void TestPersonPlusAddressAndUiHintsWithForm()
        {
            var testMethod1 = Utilities.GetTestDataFileContents("CSharpToTypeScript.TestNamespace.TestMethod1");
            var testMethod2 = Utilities.GetTestDataFileContents("CSharpToTypeScript.TestNamespace.TestMethod2");

            var testMethod1Path = Path.Combine(Utilities.GetProjectFolder(), @"TestData\src\CSharpToTypeScript.TestNamespace.TestMethod1.ts");
            var testMethod2Path = Path.Combine(Utilities.GetProjectFolder(), @"TestData\src\CSharpToTypeScript.TestNamespace.TestMethod2.ts");

            MethodInfo fileContainsScript = typeof(TypeScript).Assembly.GetType("Program")!.GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .First(m => m.Name == "<<Main>$>g__FileContainsScript|0_1");

            Assert.AreEqual(true, fileContainsScript.Invoke(null, new object[] { testMethod1, testMethod2Path }));
            Assert.AreEqual(false, fileContainsScript.Invoke(null, new object[] { testMethod2, testMethod1Path }));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
