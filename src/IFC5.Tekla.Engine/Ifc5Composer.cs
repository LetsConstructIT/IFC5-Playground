using IFC5Tekla.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC5Tekla.Engine;
internal class Ifc5Composer
{
    private readonly string _childSeparator = "__";
    public void Compose(List<PrimJson> prims)
    {
        var parentChildrenDictionary = new Dictionary<string, List<string>>();
        foreach (var prim in prims)
        {
            if (prim is not IParent parent || parent.Children is null)
                continue;

            var primName = prim.Name!;
            var childNames = parent.Children.Select(c => GetChildName(prim, c)).ToList();
            parentChildrenDictionary[primName] = childNames;
        }

        //inherits
        foreach (var prim in prims)
        {
            var inheritName = prim.GetValidInherit();
            if (inheritName is not null)
            {
                var primName = prim.Name!;
                if (parentChildrenDictionary.ContainsKey(primName))
                    parentChildrenDictionary[primName].Add(inheritName);
                else
                    parentChildrenDictionary[primName] = new List<string>() { inheritName };
            }

            if (prim is not IParent parent || parent.Children is null)
                continue;

            foreach (var child in parent.Children)
            {
                var childInheritName = child.GetValidInherit();
                if (childInheritName is null)
                    continue;

                var childName = GetChildName(prim, child);
                if (parentChildrenDictionary.ContainsKey(childName))
                    parentChildrenDictionary[childName].Add(childInheritName);
                else
                    parentChildrenDictionary[childName] = new List<string>() { childInheritName };
            }
        }

        var validInherits = prims.Select(p => p.GetValidInherit()).Where(i => i != null).ToList();
        foreach (var parent in prims.OfType<IParent>())
        {
            if (parent.Children is null)
                continue;

            foreach (var child in parent.Children)
            {
                var inherit = child.GetValidInherit();
                if (inherit != null)
                    validInherits.Add(inherit);
            }
        }

        var classes = prims.Where(p => p is ClassJson).ToList();
        var defs = prims.Where(p => p is DefJson).ToList();
        var overs = prims.Where(p => p is OverJson).ToList();


        var dictionary = Flatten(prims);

        var names = defs.Select(p => p.Name).ToHashSet();
        foreach (var prim in prims.Where(p => p is IParent))
        {
            var primName = prim.Name!;
            names.Add(primName);

            if (prim is not IParent parent || parent.Children is null)
                continue;

            foreach (var child in parent.Children)
            {
                names.Add($"{primName}__{child.Name}");
            }
        }

        var count = dictionary.Count;
    }

    private string GetChildName(PrimJson parent, DefJson child)
    {
        return $"{parent.Name}{_childSeparator}{child.Name}";
    }

    private static Dictionary<string, List<PrimJson>> Flatten(List<PrimJson> prims)
    {
        var dictionary = new Dictionary<string, List<PrimJson>>();
        foreach (var prim in prims)
        {
            if (prim is null || string.IsNullOrEmpty(prim.Name)) continue;

            var name = prim.Name!;
            if (dictionary.ContainsKey(name))
                dictionary[name].Add(prim);
            else
                dictionary[name] = new List<PrimJson>() { prim };
        }

        return dictionary;
    }
}
