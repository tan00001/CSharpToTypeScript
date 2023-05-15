using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CSharpToTypeScript.AlternateGenerators;
using TestAssembly;

namespace CSharpToTypeScript.Test.TsClass
{
    [DataContract(Name = "Person")]
    public class PersonPlusCustomValidator
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "12" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "12" })]
        public string? LastName { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Street Address", Order = 3)]
        [UIHint("", "HTML", new object[] { "colSpan", "12" })]
        public string? StreetAddress { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Order = 4)]
        [UIHint("", "HTML", new object[] { "colSpan", "6" })]
        public string? City { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Order = 5)]
        [UIHint("", "HTML", new object[] { "colSpan", "3" })]
        public string? State { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "ZIP", Order = 6)]
        [UIHint("", "HTML", new object[] { "colSpan", "3" })]
        [CustomValidation(typeof(PersonPlusCustomValidator), nameof(ValidateBalance))]
        [CustomValidation(typeof(PersonPlusCustomValidator), nameof(ValidateBalance2))]
        public string? Zip { get; set; }

        public virtual ValidationResult ValidateBalance(object value)
        {
            return new ValidationResult(value != null ? null : "Value cannot be null");
        }

        public virtual ValidationResult ValidateBalance2(object value, ValidationContext context)
        {
            return new ValidationResult(value != null ? null : "Value cannot be null");
        }
    }

    [TestClass]
    public class TestPersonPlusCustomValidatorWithForm : IDisposable
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
        public void TestPersonAndAddressAndUiHintsWithForm()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(12, false))
                .For<PersonPlusCustomValidator>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(PersonPlusCustomValidator));

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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
