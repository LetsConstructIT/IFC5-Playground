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
        var ifcX = JsonConvert.DeserializeObject<Root>(File.ReadAllText(path), Converter.Settings);

        var test = ifcX.Where(i => i != null && i.Name == "SpaceMaterial").ToList();
        Console.WriteLine(ifcX.Count());
        Console.ReadLine();
    }
}
