using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public struct PersonWithArrayMember
    {
        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public Int32 Id { get; set; }

        public string[] Notes { get; set; }
    }

    [TestClass]
    public class TestIPersonWithArrayMember : IDisposable
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
        public void TestStructureWithArrayMember()
        {
            var ts = TypeScript.Definitions(new TsGenerator(false))
               .For<PersonWithArrayMember>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithArrayMember));

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