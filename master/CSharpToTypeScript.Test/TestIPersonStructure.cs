using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public struct PersonStructure
    {
        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public Int32 Id { get; set; }

        [StringLength(10024)]
        public string Description { get; set; }
    }

    public struct PersonStructure<T>
    {
        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public Int32 Id { get; set; }

        [StringLength(10024)]
        public string Description { get; set; }

        public T TestVaue { get; set; }
    }

    public struct PersonStructureWithTestAge : IComparable
    {
        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public Int32 Id { get; set; }

        [StringLength(10024)]
        public string Description { get; set; }

        public Int32 Age { get; set; }

        public int CompareTo(object? obj)
        {
            if (obj is not PersonStructureWithTestAge personStructureWithTestAge)
            {
                return 1;
            }

            return Age.CompareTo(personStructureWithTestAge.Age);
        }
    }

    public struct PersonStructureWithField
    {
        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public Int32 Id { get; set; }

        [StringLength(10024)]
        public string Description;
    }

    public struct PersonStructureWithConstant
    {
        public const Int32 MaxDescriptionLength = 1000;

        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public Int32 Id { get; set; }

        [StringLength(MaxDescriptionLength)]
        public string Description;
    }


    [TestClass]
    public class TestIPersonStructure : IDisposable
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
        public void TestStructure()
        {
            var ts = TypeScript.Definitions(new TsGenerator(false))
               .For<PersonStructure>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonStructure));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestStructureWithInterface()
        {
            var ts = TypeScript.Definitions(new TsGenerator(false))
               .For<PersonStructureWithTestAge>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonStructureWithTestAge));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestStructureWithStringParam()
        {
            var ts = TypeScript.Definitions(new TsGenerator(false))
               .For<PersonStructure<string>>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(typeof(PersonStructure<>).Name + "[[System.String]]");

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestStructureWithResolver()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonStructure>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestResolverFileContents(nameof(PersonStructure));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestStructureWithForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
               .For<PersonStructure>();

            var personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonStructure));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestStructureFormWithField()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
               .For<PersonStructureWithField>();

            var result = ts.Generate(TsGeneratorOptions.Fields | TsGeneratorOptions.Properties | TsGeneratorOptions.Enums)
                .Where(r => !r.Value.ExcludeFromResultToString).First();

            string personTypeScript = result.Value.Script;

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonStructureWithField));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestStructureFormWithConstant()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, false))
               .For<PersonStructureWithConstant>();

            var result = ts.Generate(TsGeneratorOptions.Enums | TsGeneratorOptions.Constants)
                .Where(r => !r.Value.ExcludeFromResultToString).First();

            string personTypeScript = result.Value.Script;

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonStructureWithConstant));

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