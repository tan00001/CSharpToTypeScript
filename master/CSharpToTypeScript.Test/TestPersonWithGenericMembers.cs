using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public class PersonWithGenericMember<T> where T: new()
    {
        [UIHint("hidden")]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(20, 120)]
        public Int32? Age;

        [StringLength(120, MinimumLength = 2)]
        public string? Location;

        public T TestMember { get; set; }

        public PersonWithGenericMember(T testMember)
        {
            TestMember = testMember;
        }
    }


    [TestClass]
    public class TestPersonWithGenericMembers : IDisposable
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
        public void TestGenericDefinition()
        {
            var ts = TypeScript.Definitions(new TsGenerator(false))
               .For(typeof(PersonWithGenericMember<>));

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(typeof(PersonWithGenericMember<>).Name);

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestGenericDefinitionWithResolver()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithGenericMember<DateTimeOffset>>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestResolverFileContents(nameof(PersonWithGenericMember<DateTimeOffset>));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestGenericDefinitionForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(3, false))
               .For<PersonWithGenericMember<DateTimeOffset>>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonWithGenericMember<DateTimeOffset>));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestGenericDefinitionFormWithParam()
        {
            var typeName = "CSharpToTypeScript.Test.PersonWithGenericMember`1[[CSharpToTypeScript.Test.RegistrationStatus]]";
            var filePath = Path.Combine(this.TestContext.TestRunDirectory!, typeName + ".d.tsx");

            CSharpToTypeScriptProgram.Run(typeName, filePath, "withform(1)");

            var personTypeScript = File.ReadAllText(filePath);

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents("PersonWithGenericMember`1[[CSharpToTypeScript.Test.RegistrationStatus]]");

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