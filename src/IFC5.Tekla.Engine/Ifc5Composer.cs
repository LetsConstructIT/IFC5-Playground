using IFC5Tekla.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC5Tekla.Engine;
internal class Ifc5Composer
{
    public void Compose(List<Prim> prims)
    {
        var dictionary = Flatten(prims);

        var count = dictionary.Count;
    }

    private static Dictionary<string, List<Prim>> Flatten(List<Prim> prims)
    {
        var dictionary = new Dictionary<string, List<Prim>>();
        foreach (var prim in prims)
        {
            if (prim is null || string.IsNullOrEmpty(prim.Name)) continue;

            var name = prim.Name!;
            if (dictionary.ContainsKey(name))
                dictionary[name].Add(prim);
            else
                dictionary[name] = new List<Prim>() { prim };
        }

        return dictionary;
    }
}
