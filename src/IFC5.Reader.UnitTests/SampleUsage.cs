namespace IFC5.Reader.UnitTests;

[TestClass]
public class SampleUsage
{
    [TestMethod]
    public void ReadingFromFile()
    {
        var path = GetSamplesPath("hello-wall.ifcx");

        var composedObjects = new Reader().Read(path);
    }

    private string GetSamplesPath(string fileName)
    {
        var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        var path = Path.Combine(directory, "Samples", fileName);
        return path;
    }
}