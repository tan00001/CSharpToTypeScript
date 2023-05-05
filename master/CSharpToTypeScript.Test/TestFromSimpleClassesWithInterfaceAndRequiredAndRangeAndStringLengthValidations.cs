using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test
{
    public interface IPersonWithValidation
    {
        int Id { get; set; }

        string Name { get; set; }

        Int32? Age { get; set; }

        string? Location { get; set; }
    }

    public class PersonWithValidationAndInterface : IPersonWithValidation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(20, 120)]
        public Int32? Age { get; set; }

        [StringLength(120, MinimumLength = 2)]
        public string? Location { get; set; }
    }

    public interface IPersonWithGenderAndValidation
    {
        Gender? Gender { get; set; }
    }

    public interface IPersonWithGenderAndValidation2 : IPersonWithValidation
    {
        Gender? Gender { get; set; }
    }

    public class PersonWithGenderAndValidationInterface: PersonWithValidationAndInterface, IPersonWithGenderAndValidation
    {
        [Required]
        public Gender? Gender { get; set; }
    }

    public class PersonWithGenderAndValidationInterface2 : PersonWithValidationAndInterface, IPersonWithGenderAndValidation2
    {
        [Required]
        public Gender? Gender { get; set; }
    }

    [TestClass]
    public class TestFromSimpleClassesWithInterfaceAndRequiredAndRangeAndStringLengthValidations : IDisposable
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
        public void TestGenerateTypeScriptFileForSimpleClassesWithRequiredAndRangeAndStringLengthValidations()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithValidationAndInterface>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithValidationAndInterface));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleClassesWithValidationsWithEnum()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithGenderAndValidationInterface>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithGenderAndValidationInterface));

            Assert.AreEqual(expectedData, script);
        }

        [TestMethod]
        public void TestGenerateTypeScriptFileForSimpleClassesWithValidationsWithEnum2()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithGenderAndValidationInterface2>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithGenderAndValidationInterface2));

            Assert.AreEqual(expectedData, script);
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