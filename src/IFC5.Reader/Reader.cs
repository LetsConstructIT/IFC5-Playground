using IFC5.Reader.Composers;
using IFC5.Reader.Models;
using IFC5.Reader.Models.DTOs;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace IFC5.Reader;

public class Reader
{
    private readonly Composer _ifc5Composer = new();

    public ComposedObjects Read(string path)
    {
        var ifcContent = JsonConvert.DeserializeObject<Root>(File.ReadAllText(path), Ifc5JsonConverter.Settings)!;

        var composedObjects = _ifc5Composer.Compose(ifcContent.Where(i => i is not null));

        return composedObjects;
    }
}
