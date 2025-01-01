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
        var ifcX = JsonConvert.DeserializeObject<ModelsNew.Root>(File.ReadAllText(path), ModelsNew.Converter.Settings);

        var test = ifcX.Where(i => i != null && i.Name == "N25503984660543a18597eae657ff5bea_Body").ToList();
        Console.WriteLine(ifcX.Count());
        Console.ReadLine();
    }


    public void ReadOld(string path)
    {
        var ifcX = JsonConvert.DeserializeObject<List<IfcX>>(File.ReadAllText(path), Converter.Settings);

        var disclaimer = ifcX.FirstOrDefault(i => i.Disclaimer is not null);
        var prims = ifcX.Where(i => i.PrimType is not null).ToList();

        var dictionary = new Dictionary<string, List<IfcX>>();
        foreach (var prim in prims)
        {
            if (string.IsNullOrEmpty(prim.Name))
                continue;

            if (dictionary.ContainsKey(prim.Name))
                dictionary[prim.Name].Add(prim);
            else
                dictionary[prim.Name] = new List<IfcX>() { prim };
        }

        var count = dictionary.Count;
    }

}
