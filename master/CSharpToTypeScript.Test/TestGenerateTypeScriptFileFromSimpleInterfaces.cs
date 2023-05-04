using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Test
{
    public interface IPerson
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTime DateOfBirth { get; set; }
    }

    public interface IPersonWithGender : IPerson
    {
        Gender Gender { get; set; }
    }

    [TestClass]
    public class TestGenerateTypeScriptFileFromSimpleInterfaces : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleInterface()
        {
            var ts = TypeScript.Definitions()
               .For<IPerson>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            Assert.AreEqual("interface IPerson {\r\n\tdateOfBirth?: number;\r\n\tid?: number;\r\n\tname?: string;\r\n}\r\n\r\n",
                personTypeScript);

        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleInterfaceWithEnum()
        {
            var ts = TypeScript.Definitions()
               .For<IPersonWithGender>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("export const enum Gender {\r\n\tUnknown = 0,\r\n\tMale = 1,\r\n\tFemale = 2\r\n}\r\n\r\ninterface IPerson {\r\n\tdateOfBirth?: number;\r\n\tid?: number;\r\n\tname?: string;\r\n}\r\n\r\ninterface IPersonWithGender extends IPerson {\r\n\tgender?: Gender;\r\n}\r\n\r\n",
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