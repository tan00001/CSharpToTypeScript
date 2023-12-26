using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test.TsInterface
{
    public enum Gender
    {
        Unknown = 0,
        Male = 1,
        Female = 2
    }

    public interface IPersonWithValidation
    {
        int Id { get; set; }

        string Name { get; set; }

        int? Age { get; set; }

        string? Location { get; set; }
    }

    public class PersonWithValidationAndInterface : IPersonWithValidation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Range(20, 120)]
        public int? Age { get; set; }

        [StringLength(120, MinimumLength = 2)]
        public string? Location { get; set; }
    }

    public interface IPersonWithGenderAndValidation
    {
        Gender? Gender { get; set; }
    }

    public class PersonWithGenderAndValidationInterface : PersonWithValidationAndInterface, IPersonWithGenderAndValidation
    {
        [Required]
        public Gender? Gender { get; set; }
    }

    [TestClass]
    public class TestPersonWithValidationAndInterfaceWithResolverAndForm : IDisposable
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
        public void TestPersonWithValidationAndInterfaceWithResolver()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithValidationAndInterface>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithValidationAndInterface));

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestPersonWithValidationAndInterfaceWithForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(3, null, false))
               .For<PersonWithValidationAndInterface>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents("PersonWithValidationAndInterface");

            Assert.AreEqual(expectedData, personTypeScript);
        }

        [TestMethod]
        public void TestPersonWithGenderAndValidationInterfaceWithResolver()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
               .For<PersonWithGenderAndValidationInterface>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestDataFileContents(nameof(PersonWithGenderAndValidationInterface));

            Assert.AreEqual(expectedData, script);
        }

        [TestMethod]
        public void TestPersonWithGenderAndValidationInterfaceWithForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(3, null, false))
               .For<PersonWithGenderAndValidationInterface>();

            string script = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(script));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonWithGenderAndValidationInterface));

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