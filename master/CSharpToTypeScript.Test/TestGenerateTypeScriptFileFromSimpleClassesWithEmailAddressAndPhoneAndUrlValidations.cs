using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public class PersonWithEmailAddressAndPhoneAndSiteUrl
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string? EmailAddress { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Url]
        public string? HomePage { get; set; }
	}

    [TestClass]
    public class TestGenerateTypeScriptFileFromSimpleClassesWithEmailAddressAndPhoneAndUrlValidations : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleClassesWithEmailAddressAndPhoneAndUrlValidations()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithEmailAddressAndPhoneAndSiteUrl>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithEmailAddressAndPhoneAndSiteUrl));

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