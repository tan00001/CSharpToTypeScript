using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Test
{
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class PersonWithGender : Person
    {
        public Gender Gender { get; set; }
    }

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
            var filePath = Path.Combine(this.TestContext.TestRunDirectory, "Person.d.ts");

            CSharpToTypeScriptProgram.Run("CSharpToTypeScript.Test.Person", filePath);

            var personTypeScript = File.ReadAllText(filePath);

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            Assert.AreEqual("/// <reference path=\"\" />\r\n\r\ndeclare namespace CSharpToTypeScript.Test {\r\n\tinterface Person {\r\n\t\tDateOfBirth?: number;\r\n\t\tId?: number;\r\n\t\tName?: string;\r\n\t}\r\n}\r\n",
                personTypeScript.Replace(filePath, ""));

        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleClassWithEnum()
        {
            var filePath = Path.Combine(this.TestContext.TestRunDirectory, "PersonWithGender.d.ts");
            var ts = TypeScript.Definitions()
               .WithReference(filePath)
               .For<PersonWithGender>();

            string script = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("/// <reference path=\"\" />\r\n\r\ndeclare namespace CSharpToTypeScript.Test {\r\n\texport const enum Gender {\r\n\t\tUnknown = 0,\r\n\t\tMale = 1,\r\n\t\tFemale = 2\r\n\t}\r\n\tinterface Person {\r\n\t\tDateOfBirth?: number;\r\n\t\tId?: number;\r\n\t\tName?: string;\r\n\t}\r\n\tinterface PersonWithGender extends CSharpToTypeScript.Test.Person {\r\n\t\tGender?: CSharpToTypeScript.Test.Gender;\r\n\t}\r\n}\r\n",
                script.Replace(filePath, ""));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TestGenerateTypeScriptFile()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}