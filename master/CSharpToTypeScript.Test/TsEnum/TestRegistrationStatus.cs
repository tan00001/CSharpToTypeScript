using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;
using CSharpToTypeScript.Test.TsClass;

namespace CSharpToTypeScript.Test.TsEnum
{
    [TestClass]
    public class TestRegistrationStatus : IDisposable
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
        public void TestEnum()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<RegistrationStatus>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(RegistrationStatus));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestNullableEnum()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<RegistrationStatus?>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(RegistrationStatus));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}