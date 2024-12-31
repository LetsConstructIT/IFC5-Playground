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
