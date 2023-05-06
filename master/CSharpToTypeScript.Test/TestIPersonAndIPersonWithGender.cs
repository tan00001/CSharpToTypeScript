using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Test
{
    public interface IPerson
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime DateOfBirth { get; set; }
    }

    public interface IPersonWithGender : IPerson
    {
        Gender Gender { get; set; }
    }

    [TestClass]
    public class TestIPersonAndIPersonWithGender : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleInterface()
        {
            var ts = TypeScript.Definitions()
               .For<IPerson>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(IPerson));

            Assert.AreEqual(expectedData, personTypeScript);

        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleInterfaceWithEnum()
        {
            var ts = TypeScript.Definitions()
               .For<IPersonWithGender>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(IPersonWithGender));

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