using IFC5Tekla.Engine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IFC5Tekla.Engine;

public class Ifc5Reader
{
    public void Read(string path)
    {
        var ifcContent = JsonConvert.DeserializeObject<Root>(File.ReadAllText(path), Converter.Settings)!;

        new Ifc5Composer().Compose(ifcContent.Where(i => i is not null).ToList());
        Console.WriteLine(ifcContent.Count());
        Console.ReadLine();
    }
}
