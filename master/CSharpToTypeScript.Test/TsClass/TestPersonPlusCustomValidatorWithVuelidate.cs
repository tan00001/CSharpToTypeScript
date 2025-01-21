using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test.TsClass
{
    [TestClass]
    public class TestPersonPlusCustomValidatorWithVuelidate : IDisposable
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
        public void TestPersonAndAddressAndUiHintsWithVuelidate()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithVuelidate(false))
                .For<PersonPlusCustomValidator>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestVuelidateRuleContents("TsClass", nameof(PersonPlusCustomValidator));
            var expectedDataRelease = Utilities.GetTestVuelidateRuleContents("TsClass", nameof(PersonPlusCustomValidator) + "Release");

            Assert.IsTrue(expectedData == personTypeScript || expectedDataRelease == personTypeScript);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
