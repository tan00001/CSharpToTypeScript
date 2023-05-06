using System;
using System.Runtime.Serialization;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    [DataContract(Namespace = "CSharpToTypeScript.TestNamespace.Enums")]
    public enum RegistrationStatus
    {
        Unknown,
        Registered,
        Unregistered
    }


    [DataContract(Namespace = "CSharpToTypeScript.TestNamespace")]
    public class PersonWithNamespace : PersonWithNullableName
    {
        public Gender? Gender { get; set; }

        public RegistrationStatus? Status { get; set; }
    }

    [TestClass]
    public class TestPersonWithNamespace : IDisposable
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
        public void TestGenerateTypeScriptFileForPersonWithCustomNamespace()
        {
            var ts = TypeScript.Definitions()
               .For<PersonWithNamespace>();

            var results = ts.Generate();

            Assert.AreEqual(3, results.Count);

            foreach (var result in results)
            {
                if (result.Key == "CSharpToTypeScript.TestNamespace")
                {
                    var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithNamespace));
                    Assert.AreEqual(expectedData, result.Value);
                }
                else
                {
                    var expectedData = Utilities.GetTestDataFileContents(result.Key);
                    Assert.AreEqual(expectedData, result.Value);
                }
            }
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