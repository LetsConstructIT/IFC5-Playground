using IFC5.Reader.Composers;
using IFC5.Reader.Models;
using IFC5.Reader.Models.DTOs;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace IFC5.Reader;

public class Reader
{
    private readonly Ifc5JsonConverter _ifc5JsonConverter = new();
    private readonly Composer _ifc5Composer = new();

    public ComposedObjects Read(string path)
    {
        var ifcContent = _ifc5JsonConverter.Deserialize(File.ReadAllText(path));

        var composedObjects = _ifc5Composer.Compose(ifcContent.Where(i => i is not null));

        return composedObjects;
    }
}
