using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test.TsClass
{
    public class PersonWithConstant
    {
        public const int MaxDescriptionLength = 1000;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [StringLength(MaxDescriptionLength)]
        public string Description = string.Empty;
    }


    [TestClass]
    public class TestPersonStructure : IDisposable
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
        public void TestStructureFormWithConstant()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
               .For<PersonWithConstant>();

            var result = ts.Generate(TsGeneratorOptions.Enums | TsGeneratorOptions.Constants)
                .Where(r => !r.Value.ExcludeFromResultToString).First();

            string personTypeScript = result.Value.Script;

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents("TsClass", nameof(PersonWithConstant));

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