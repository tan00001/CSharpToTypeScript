using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public class PersonWithFieldValues
    {
        [UIHint("hidden")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(20, 120)]
        public Int32? Age;

        [StringLength(120, MinimumLength = 2)]
        public string? Location;
    }

    public class PersonWithGenderFieldValue: PersonWithValidation
    {
        [Required]
        public Gender? Gender;
    }

    [TestClass]
    public class TestPersonWithFieldValues : IDisposable
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
        public void TestFieldValues()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithFieldValues>();

            var result = ts.Generate(TsGeneratorOptions.Fields | TsGeneratorOptions.Properties | TsGeneratorOptions.Enums)
                .Where(r => !r.Value.ExcludeFromResultToString).First();

            string personTypeScript = result.Value.Script;

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithFieldValues));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestFieldValuesForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(3, false))
               .For<PersonWithGenderFieldValue>();

            var result = ts.Generate(TsGeneratorOptions.Fields | TsGeneratorOptions.Properties | TsGeneratorOptions.Enums)
                .Where(r => !r.Value.ExcludeFromResultToString).First();

            string personTypeScript = result.Value.Script;
            
            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonWithGenderFieldValue));

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