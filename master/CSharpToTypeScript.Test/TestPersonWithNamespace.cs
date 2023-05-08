using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    [DataContract(Namespace = "CSharpToTypeScript.TestNamespace.Enums")]
    public enum RegistrationStatus
    {
        Unknown,
        Registered,
        Unregistered,
        [Display(Name = "Approval Pending")]
        ApprovalPending
    }


    [DataContract(Namespace = "CSharpToTypeScript.TestNamespace")]
    public class PersonWithNamespace : PersonWithNullableName
    {
        public Gender? Gender { get; set; }

        public RegistrationStatus? Status { get; set; }
    }

    [TestClass]
    public class TestPersonWithNamespace : IDisposable
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
        public void TestGenerateTypeScriptFileForPersonWithCustomNamespace()
        {
            var ts = TypeScript.Definitions()
               .For<PersonWithNamespace>();

            var results = ts.Generate();

            Assert.AreEqual(3, results.Count);

            foreach (var result in results)
            {
                if (result.Key == "CSharpToTypeScript.TestNamespace")
                {
                    var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithNamespace));
                    Assert.AreEqual(expectedData, result.Value.Script);
                }
                else
                {
                    var expectedData = Utilities.GetTestDataFileContents(result.Key);
                    Assert.AreEqual(expectedData, result.Value.Script);
                }
            }
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForPersonWithCustomNamespaceAndForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(3, false))
               .For<PersonWithNamespace>();

            var results = ts.Generate();

            Assert.AreEqual(4, results.Count);

            foreach (var result in results)
            {
                string expectedData;
                if (result.Key == "CSharpToTypeScript.TestNamespace")
                {
                    if (result.Value.FileType == TsGeneratorWithForm.TypeScriptXmlFileType)
                    {
                        expectedData = Utilities.GetTestFormFileContents(nameof(PersonWithNamespace));
                    }
                    else
                    {
                        expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithNamespace));
                    }
                }
                else
                {
                    if (result.Value.FileType == TsGeneratorWithForm.TypeScriptXmlFileType)
                    {
                        expectedData = Utilities.GetTestFormFileContents(result.Key);
                    }
                    else
                    {
                        expectedData = Utilities.GetTestDataFileContents(result.Key);
                    }
                }
                Assert.AreEqual(expectedData, result.Value.Script);
            }
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

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}