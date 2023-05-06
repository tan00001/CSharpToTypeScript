using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public class PersonWithValidation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(20, 120)]
        public Int32? Age { get; set; }

        [StringLength(120, MinimumLength = 2)]
        public string? Location { get; set; }
    }

    public class PersonWithGengerAndValidation: PersonWithValidation
    {
        [Required]
        public Gender? Gender { get; set; }
    }

    [TestClass]
    public class TestPersonWithValidationWithResolver : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleClassesWithRequiredAndRangeAndStringLengthValidations()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithValidation>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithValidation));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleClassesWithRequiredAndRangeAndStringLengthForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(3, false))
               .For<PersonWithValidation>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonWithValidation));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleClassesWithValidationsWithEnum()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithGengerAndValidation>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithGengerAndValidation));

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