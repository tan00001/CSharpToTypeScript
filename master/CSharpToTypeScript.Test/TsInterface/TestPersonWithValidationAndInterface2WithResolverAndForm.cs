using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;
using CSharpToTypeScript.Test.TsClass;

namespace CSharpToTypeScript.Test.TsInterface
{
    public interface IPersonWithGenderAndValidation2 : IPersonWithValidation
    {
        Gender? Gender { get; set; }
    }

    public class PersonWithGenderAndValidationInterface2 : PersonWithValidationAndInterface, IPersonWithGenderAndValidation2
    {
        [Required]
        public Gender? Gender { get; set; }
    }

    [TestClass]
    public class TestPersonWithValidationAndInterface2WithResolverAndForm : IDisposable
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
        public void TestPersonWithValidationAndInterface2WithResolver()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithGenderAndValidationInterface2>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithGenderAndValidationInterface2));

            Assert.AreEqual(expectedData, script);
        }

        [TestMethod]
        public void TestPersonWithValidationAndInterface2WithForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(5, null, false))
               .For<PersonWithGenderAndValidationInterface2>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonWithGenderAndValidationInterface2));

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