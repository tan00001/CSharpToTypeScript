using System;
using System.Runtime.Serialization;

namespace CSharpToTypeScript.Test
{

    [DataContract(Namespace = "CSharpToTypeScript.TestNamespace")]
    public class PersonWithNamespace : PersonWithNullableName
    {
        public Gender? Gender { get; set; }
    }

    [TestClass]
    public class TestGenerateTypeScriptFileWithCustomNamespace : IDisposable
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

            string script = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("namespace CSharpToTypeScript.Test {\r\n\texport const enum Gender {\r\n\t\tUnknown = 0,\r\n\t\tMale = 1,\r\n\t\tFemale = 2\r\n\t}\r\n\tclass PersonWithNullableName {\r\n\t\tage?: number | null;\r\n\t\tdateOfBirth?: number;\r\n\t\tid?: number;\r\n\t\tname?: string | null;\r\n\t}\r\n}\r\nnamespace CSharpToTypeScript.TestNamespace {\r\n\tclass PersonWithNamespace extends CSharpToTypeScript.Test.PersonWithNullableName {\r\n\t\tgender?: CSharpToTypeScript.Test.Gender | null;\r\n\t}\r\n}\r\n",
                script);
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