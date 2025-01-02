﻿using IFC5Tekla.Engine.Domain;
using IFC5Tekla.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFC5Tekla.Engine;
internal class TreeFlattener
{
    private readonly IEnumerable<PrimJson> _jsonPrims;
    private readonly Dictionary<string, ChildNames> _relations = new();

    private readonly string _childSeparator = "__";

    public TreeFlattener(IEnumerable<PrimJson> prims)
    {
        _jsonPrims = prims ?? throw new ArgumentNullException(nameof(prims));
    }

    public FlattenedTree Flatten()
    {
        _relations.Clear();

        var prims = CollectTopLevelPrims();
        prims.AddRange(CollectChildrenPrims());

        AddInheritRelations(prims);

        return new FlattenedTree(prims, _relations);
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

    private void AddRelation(string parentName, string childName)
    {
        if (_relations.ContainsKey(parentName))
            _relations[parentName].Add(childName);
        else
            _relations[parentName] = new ChildNames { childName };
    }

    private void AddInheritRelations(List<Prim> prims)
    {
        foreach (var parent in prims.OfType<Def>().Where(p => p.Inherits.Length > 0))
        {
            foreach (var inherit in parent.Inherits.Select(GetInheritName))
                AddRelation(parent.Name, inherit);
        }

        foreach (var parent in prims.OfType<Class>().Where(p => p.Inherits.Length > 0))
        {
            foreach (var inherit in parent.Inherits.Select(GetInheritName))
                AddRelation(parent.Name, inherit);
        }
    }

    private string GetChildName(PrimJson parent, DefJson child)
    {
        return $"{parent.Name}{_childSeparator}{child.Name}";
    }

    public string GetInheritName(string input)
    {
        var offset = 2; // first two chars are </
        return input.Substring(offset, input.Length - offset - 1);
    }
}

public class FlattenedTree
{
    public IReadOnlyList<Prim> Prims { get; }
    public Dictionary<string, ChildNames> Relations { get; }

    public FlattenedTree(IReadOnlyList<Prim> prims, Dictionary<string, ChildNames> _relations)
    {
        Prims = prims ?? throw new ArgumentNullException(nameof(prims));
        Relations = _relations ?? throw new ArgumentNullException(nameof(_relations));
    }

    public HashSet<string> FindRoots()
    {
        var roots = Prims.Where(p => p is Class || p is Def).Select(p => p.Name).ToHashSet();
        foreach (var root in Relations)
        {
            foreach (var childName in root.Value)
                roots.Remove(childName);
        }

        return roots;
    }
}

public class ChildNames : List<string>
{

}