using System;

namespace CSharpToTypeScript.Test.TsClass
{
    public class PersonWithNullableName
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int? Age { get; set; }
    }

    public class PersonWithNullableGender : PersonWithNullableName
    {
        public Gender? Gender { get; set; }
    }

    [TestClass]
    public class TestPersonWithNullableNameAndPersonWithNullableGender : IDisposable
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
        public void TestGenerateTypeScriptFileForPersonWithNullableName()
        {
            var ts = TypeScript.Definitions()
               .For<PersonWithNullableName>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithNullableName));

            Assert.AreEqual(expectedData, script);
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForPersonWithNullableGender()
        {
            var ts = TypeScript.Definitions()
               .For<PersonWithNullableGender>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithNullableGender));

            Assert.AreEqual(expectedData, script);
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