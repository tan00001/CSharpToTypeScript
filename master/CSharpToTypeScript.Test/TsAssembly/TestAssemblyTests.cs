using System.ComponentModel.DataAnnotations;
using CSharpToTypeScript.AlternateGenerators;

namespace CSharpToTypeScript.Test.TsAssembly;

[TestClass]
public class TestAssemblyTests : IDisposable
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