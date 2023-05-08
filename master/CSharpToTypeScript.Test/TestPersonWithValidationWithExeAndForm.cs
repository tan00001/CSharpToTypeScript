using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    [TestClass]
    public class TestPersonWithValidationWithExeAndForm : IDisposable
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
        public void TestPersonWithValidationWithExeAndRequiredAndRangeAndStringLengthAndGenerateForm()
        {
            var filePath = Path.Combine(this.TestContext.TestRunDirectory!, "PersonWithValidation.d.tsx");

            CSharpToTypeScriptProgram.Run("CSharpToTypeScript.Test.PersonWithValidation", filePath, "withform(3)");

            var personTypeScript = File.ReadAllText(filePath);

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonWithValidation));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestPersonWithGenderAndValidationWithEnumAndGenerateForm()
        {
            var filePath = Path.Combine(this.TestContext.TestRunDirectory!, "PersonWithGenderAndValidation.d.tsx");

            CSharpToTypeScriptProgram.Run("CSharpToTypeScript.Test.PersonWithGenderAndValidation", filePath, "withform(3)");

            var personTypeScript = File.ReadAllText(filePath);

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonWithGenderAndValidation));

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