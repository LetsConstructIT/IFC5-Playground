using IFC5.Reader.Composers;
using IFC5.Reader.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace IFC5.Reader;

public class Reader
{
    public ComposedObjects Read(string path)
    {
        var ifcContent = JsonConvert.DeserializeObject<Root>(File.ReadAllText(path), Converter.Settings)!;

        var composedObjects = new Composer().Compose(ifcContent.Where(i => i is not null).ToList());

        return composedObjects;
    }
}
