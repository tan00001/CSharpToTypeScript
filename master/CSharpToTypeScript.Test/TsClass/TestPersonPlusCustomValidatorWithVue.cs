using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CSharpToTypeScript.AlternateGenerators;
using TestAssembly;

namespace CSharpToTypeScript.Test.TsClass
{
    [TestClass]
    public class TestPersonPlusCustomValidatorWithVue : IDisposable
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
        public void TestPersonAndAddressAndUiHintsWithVue()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithVue(12, null, false))
                .For<PersonPlusCustomValidator>();

            (string personTypeScript, string personVueScript)= ts.ToStringWithOtherFileTypes();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            Assert.IsTrue(!string.IsNullOrEmpty(personVueScript));

            var expectedData = Utilities.GetTestDataFileContents("TsClass", nameof(PersonPlusCustomValidator) + ".vue");
            var expectedDataRelease = Utilities.GetTestDataFileContents("TsClass", nameof(PersonPlusCustomValidator) + "Release.vue");

            Assert.IsTrue(expectedData == personTypeScript || expectedDataRelease == personTypeScript);

            var expectedVueData = Utilities.GetTestVueFileContents("TsClass", nameof(PersonPlusCustomValidator));
            var expectedVueDataRelease = Utilities.GetTestVueFileContents("TsClass", nameof(PersonPlusCustomValidator) + "Release");

            Assert.IsTrue(expectedVueData == personVueScript || expectedVueDataRelease == personVueScript);
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
