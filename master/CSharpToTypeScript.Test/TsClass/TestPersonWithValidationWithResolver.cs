using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test.TsClass
{
    public class PersonWithValidation
    {
        [UIHint("hidden")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(20, 120)]
        public int? Age { get; set; }

        [StringLength(120, MinimumLength = 2)]
        public string? Location { get; set; }
    }

    public class PersonWithGenderAndValidation : PersonWithValidation
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
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(3, null, false))
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
               .For<PersonWithGenderAndValidation>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithGenderAndValidation));

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