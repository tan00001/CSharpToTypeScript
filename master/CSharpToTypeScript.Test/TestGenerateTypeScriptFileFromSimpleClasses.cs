using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Test
{
    [TestClass]
    public class TestGenerateTypeScriptFileFromSimpleClasses : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleClass()
        {
            var ts = TypeScript.Definitions()
               .For<Person>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            Assert.AreEqual("class Person {\r\n\tdateOfBirth?: number;\r\n\tid?: number;\r\n\tname?: string;\r\n}\r\n",
                personTypeScript);

        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleClassWithEnum()
        {
            var ts = TypeScript.Definitions()
               .For<PersonWithGender>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("export const enum Gender {\r\n\tUnknown = 0,\r\n\tMale = 1,\r\n\tFemale = 2\r\n}\r\n\r\nclass Person {\r\n\tdateOfBirth?: number;\r\n\tid?: number;\r\n\tname?: string;\r\n}\r\nclass PersonWithGender extends Person {\r\n\tgender?: Gender;\r\n}\r\n",
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