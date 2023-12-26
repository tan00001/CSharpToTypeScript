using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test.TestRequiredValidationRule
{
    [DataContract(Name = "Person")]
    public class PersonRequiresAge
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Required]
        [Display(Order = 3)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public Int32? Age { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonRequiresBirthDate
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Required]
        [Display(Order = 3)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public DateTime? BirthDate { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonRequiresId
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Required]
        [Display(Order = 3)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public Guid? Id { get; set; }
    }

    [DataContract(Name = "Person")]
    public class PersonRequiresAny
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name", Order = 1)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name", Order = 2)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Time zone info is required.")]
        [Display(Order = 3)]
        [UIHint("", "HTML", new object[] { "colSpan", "1" })]
        public TimeZoneInfo? TimeZoneInfo { get; set; }
    }

    [TestClass]
    public class RequiredValidationRule
    {
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
        public void TestRequiredNumber()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, null, false))
               .For<PersonRequiresAge>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RequiredValidationRule), nameof(PersonRequiresAge));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        [TestMethod]
        public void TestRequiredDate()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, null, false))
               .For<PersonRequiresBirthDate>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RequiredValidationRule), nameof(PersonRequiresBirthDate));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        [TestMethod]
        public void TestRequiredGuid()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, null, false))
               .For<PersonRequiresId>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RequiredValidationRule), nameof(PersonRequiresId));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }

        [TestMethod]
        public void TestRequiredAny()
        {
            var ts = TypeScript.Definitions(new TsGeneratorWithForm(1, null, false))
               .For<PersonRequiresAny>();

            string personTypeScript = ts.ToString();

            Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

            var expectedData = Utilities.GetTestFormFileContents(nameof(RequiredValidationRule), nameof(PersonRequiresAny));

            Assert.AreEqual(expectedData, personTypeScript.Replace("'./BootstrapUtils'", "'../BootstrapUtils'"));
        }
    }
}
