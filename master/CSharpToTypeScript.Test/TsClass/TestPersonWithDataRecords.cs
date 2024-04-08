using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test.TsClass
{
    public record struct DataRecord
    {
        public string? Name { get; init; }

        public int Id { get; init; }
    }

    public class PersonWithDataRecords
    {
        public const int MaxDescriptionLength = 1000;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [StringLength(MaxDescriptionLength)]
        public string Description = string.Empty;

        public List<DataRecord> DataRecords { get; private set; } = new List<DataRecord>();
    }


    [TestClass]
    public class TestPersonWithDataRecords : IDisposable
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
            var ts = TypeScript.Definitions()
               .For<PersonWithDataRecords>();

            var result = ts.Generate(TsGeneratorOptions.Fields | TsGeneratorOptions.Properties | TsGeneratorOptions.Enums)
                .Where(r => !r.Value.ExcludeFromResultToString).First();

            string personTypeScript = result.Value.Script;

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents("TsClass", nameof(PersonWithDataRecords));

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