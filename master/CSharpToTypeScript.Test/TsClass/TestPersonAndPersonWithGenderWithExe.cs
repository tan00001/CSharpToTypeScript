using System;
using System.ComponentModel.DataAnnotations;

namespace CSharpToTypeScript.Test.TsClass
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
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
    }

    public class PersonWithGender : Person
    {
        public Gender Gender { get; set; }
    }

    [TestClass]
    public class TestPersonAndPersonWithGenderWithExe : IDisposable
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
        public void TestGenerateTypeScriptFileUsingExeForSimpleClasses()
        {
            var filePath = Path.Combine(TestContext.TestRunDirectory!, "Person.d.ts");

            CSharpToTypeScriptProgram.Run("CSharpToTypeScript.Test.TsClass.Person", filePath, string.Empty);

            var personTypeScript = File.ReadAllText(filePath);

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(Person));

            Assert.AreEqual(expectedData, personTypeScript);

        }

        [TestMethod]
        public void TestGenerateTypeScriptFileUsingExeForSimpleClassesWithEnum()
        {
            var filePath = Path.Combine(TestContext.TestRunDirectory!, "PersonWithGender.d.ts");

            CSharpToTypeScriptProgram.Run("CSharpToTypeScript.Test.TsClass.PersonWithGender", filePath, string.Empty);

            var personTypeScript = File.ReadAllText(filePath);

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithGender));

            Assert.AreEqual(expectedData, personTypeScript);
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