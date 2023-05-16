using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test.TsAssembly;

[TestClass]
public class TestAssemblyTests : IDisposable
{
    const string ImportReactHookForm = "import { Resolver, FieldErrors, FieldError } from 'react-hook-form';\r\n\r\n";

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
    public void TestAll()
    {
        var testAssemby = typeof(TestAssembly.Person).Assembly;

        var ts = TypeScript.Definitions(new TsGenerator(false))
           .For(testAssemby);

        var personTypeScript = ts.ToString();

        Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

        var expectedData = Utilities.GetTestDataFileContents("TsAssembly", "All");

        Assert.AreEqual(expectedData, personTypeScript);
    }

    [TestMethod]
    public void TestAllResolvers()
    {
        var testAssemby = typeof(TestAssembly.Person).Assembly;

        var ts = TypeScript.Definitions(new TsGeneratorWithResolver(false))
           .For(testAssemby);

        var personTypeScript = ts.ToString();

        Assert.IsTrue(!string.IsNullOrEmpty(personTypeScript));

        var expectedData = Utilities.GetTestFormFileContents("TsAssembly", "All");
        var expectedDataRelease = Utilities.GetTestFormFileContents("TsAssembly", "AllRelease");

        personTypeScript = ImportReactHookForm + personTypeScript.Replace('\t' + ImportReactHookForm, "");
        Assert.IsTrue(expectedData == personTypeScript || expectedDataRelease == personTypeScript);
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