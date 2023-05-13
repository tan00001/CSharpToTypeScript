using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
        public void TestGenerateFormWithInvalidAssembly()
        {
            var filePath = Path.Combine(this.TestContext.TestRunDirectory!, "InvalidTypeDefinition.d.tsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            CSharpToTypeScriptProgram.Run("InvalidAssembly.dll", "CSharpToTypeScript.Test.InvalidTypeDefinition", filePath, "withform(abc)");

            Assert.IsFalse(File.Exists(filePath));
        }


        [TestMethod]
        public void TestGenerateFormWithInvalidType()
        {
            var filePath = Path.Combine(this.TestContext.TestRunDirectory!, "InvalidTypeDefinition.d.tsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            CSharpToTypeScriptProgram.Run("CSharpToTypeScript.Test.InvalidTypeDefinition", filePath, "withform(abc)");

            Assert.IsFalse(File.Exists(filePath));
        }


        [TestMethod]
        public void TestGenerateFormWithException()
        {
            var exception = Assert.ThrowsException<TargetInvocationException>(() =>
            {
                var filePath = Path.Combine(this.TestContext.TestRunDirectory!, "PersonWithValidation.d.tsx");

                CSharpToTypeScriptProgram.Run("CSharpToTypeScript.Test.PersonWithValidation", filePath, "withform(abc)");
            });

            Assert.IsNotNull(exception.InnerException);
            Assert.IsInstanceOfType(exception.InnerException, typeof(ArgumentException));
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