using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Test
{
    [TestClass]
    public class TestPersonAndPersonWithGender : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleClass()
        {
            var ts = TypeScript.Definitions()
               .For<Person>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(Person));

            Assert.AreEqual(expectedData, personTypeScript);

        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleClassWithEnum()
        {
            var ts = TypeScript.Definitions()
               .For<PersonWithGender>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithGender));

            Assert.AreEqual(expectedData, script);
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

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}