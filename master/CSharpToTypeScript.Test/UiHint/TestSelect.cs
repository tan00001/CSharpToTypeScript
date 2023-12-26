using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;
using CSharpToTypeScript.Test.TsClass;

namespace CSharpToTypeScript.Test.TsEnum
{
    public class PersonWithSelectOptions
    {
        public static readonly string[] LocationOptions = new[] { "NY", "CA" };

        [UIHint("hidden")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(20, 120)]
        public int? Age { get; set; }

        [StringLength(120, MinimumLength = 2)]
        [UIHint("select", "HTML", new object[] { "typeContainingOptions", typeof(PersonWithSelectOptions), "nameOfOptions", nameof(LocationOptions) })]
        public string? Location { get; set; }
    }

    [TestClass]
    public class TestSelect : IDisposable
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
        public void TestSelectOptions()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, null, false))
               .For<PersonWithSelectOptions>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents("UIHint", nameof(PersonWithSelectOptions));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
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