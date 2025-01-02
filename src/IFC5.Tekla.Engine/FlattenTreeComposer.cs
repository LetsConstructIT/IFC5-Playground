using IFC5Tekla.Engine.Domain;
using IFC5Tekla.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC5Tekla.Engine;
internal class FlattenTreeComposer
{
    private readonly IEnumerable<PrimJson> _jsonPrims;
    private readonly Dictionary<string, List<string>> _relations = new();

    private readonly string _childSeparator = "__";

    public FlattenTreeComposer(IEnumerable<PrimJson> prims)
    {
        _jsonPrims = prims ?? throw new ArgumentNullException(nameof(prims));
    }

    public FlattenTree Compose()
    {
        _relations.Clear();

        var prims = CollectTopLevelPrims();
        prims.AddRange(CollectChildrenPrims());

        var classes = prims.OfType<Class>().ToList();
        var defs = prims.OfType<Def>().ToList();
        var overs = prims.OfType<Over>().ToList();
        var count = prims.Count;

        return new FlattenTree(prims);
    }

    private List<Prim> CollectTopLevelPrims()
    {
        return _jsonPrims.Select(p => p.ToDomain()).ToList();
    }

    private List<Prim> CollectChildrenPrims()
    {
        var children = new List<Prim>();
        foreach (var jsonPrim in _jsonPrims)
        {
            if (jsonPrim is not IParent parent || parent.Children is null)
                continue;

            foreach (var child in parent.Children)
            {
                var childName = GetChildName(jsonPrim, child);

                var domainChild = child.ToDomain(childName);
                children.Add(domainChild);

                AddRelation(jsonPrim.Name!, domainChild.Name);
            }
        }

        return children;
    }

    private string GetChildName(PrimJson parent, DefJson child)
    {
        return $"{parent.Name}{_childSeparator}{child.Name}";
    }

    private void AddRelation(string parentName, string childName)
    {
        if (_relations.ContainsKey(parentName))
            _relations[parentName].Add(childName);
        else
            _relations[parentName] = new List<string> { childName };
    }
}

public class FlattenTree
{
    public IReadOnlyList<Prim> Prims { get; }

    public FlattenTree(IReadOnlyList<Prim> prims)
    {
        Prims = prims ?? throw new ArgumentNullException(nameof(prims));
    }
}

