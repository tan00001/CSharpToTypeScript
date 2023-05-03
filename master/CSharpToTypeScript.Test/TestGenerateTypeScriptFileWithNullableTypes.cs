using System;

namespace CSharpToTypeScript.Test
{
    public class PersonWithNullableName
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime DateOfBirth { get; set; }

        public int? Age { get; set; }
    }

    public class PersonWithNullableGender : PersonWithNullableName
    {
        public Gender? Gender { get; set; }
    }

    [TestClass]
    public class TestGenerateTypeScriptFileWithNullableTypes : IDisposable
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
        public void TestGenerateTypeScriptFileForPersonWithNullableName()
        {
            var ts = TypeScript.Definitions()
               .For<PersonWithNullableName>();

            string script = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("class PersonWithNullableName {\r\n\tage?: number | null;\r\n\tdateOfBirth?: number;\r\n\tid?: number;\r\n\tname?: string | null;\r\n}\r\n",
                script);
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForPersonWithNullableGender()
        {
            var ts = TypeScript.Definitions()
               .For<PersonWithNullableGender>();

            string script = ts.Generate();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            Assert.AreEqual("export const enum Gender {\r\n\tUnknown = 0,\r\n\tMale = 1,\r\n\tFemale = 2\r\n}\r\nclass PersonWithNullableName {\r\n\tage?: number | null;\r\n\tdateOfBirth?: number;\r\n\tid?: number;\r\n\tname?: string | null;\r\n}\r\nclass PersonWithNullableGender extends PersonWithNullableName {\r\n\tgender?: Gender | null;\r\n}\r\n",
                script);
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