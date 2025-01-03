﻿using IFC5.Reader.Domain;
using IFC5.Reader.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFC5.Reader.Composers;
internal class TreeFlattener
{
    private readonly IEnumerable<PrimJson> _jsonPrims;
    private readonly Dictionary<string, ChildNames> _relations = new();

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
        if (_relations.TryGetValue(parentName, out ChildNames? value))
            value.Add(childName);
        else
            _relations[parentName] = new ChildNames { childName };
    }

    private void AddInheritRelations(List<Prim> prims)
    {
        foreach (var parent in prims.OfType<Def>().Where(p => p.Inherits.Length > 0))
        {
            foreach (var inherit in parent.Inherits)
                AddRelation(parent.Name, inherit);
        }

        foreach (var parent in prims.OfType<Class>().Where(p => p.Inherits.Length > 0))
        {
            foreach (var inherit in parent.Inherits)
                AddRelation(parent.Name, inherit);
        }
    }

    private string GetChildName(PrimJson parent, DefJson child)
    {
        return $"{parent.Name}{Constants.ChildSeparator}{child.Name}";
    }
}

public interface IPrimGraph
{
    IEnumerable<Prim> GetNeighbours(Prim prim);
}

public class FlattenedTree : IPrimGraph
{
    public IReadOnlyList<Prim> Prims { get; }
    public Dictionary<string, ChildNames> Relations { get; }

    private readonly Dictionary<string, Prim> _classesAndDefs;

    public FlattenedTree(IReadOnlyList<Prim> prims, Dictionary<string, ChildNames> _relations)
    {
        Prims = prims ?? throw new ArgumentNullException(nameof(prims));
        Relations = _relations ?? throw new ArgumentNullException(nameof(_relations));

        _classesAndDefs = new Dictionary<string, Prim>();
        foreach (var prim in Prims.Where(p => p is Class || p is Def))
        {
            if (_classesAndDefs.ContainsKey(prim.Name))
            {
                //TODO: should it be allowed in IFC5?
            }
            else
                _classesAndDefs.Add(prim.Name, prim);
        }
    }

    public IReadOnlyList<string> FindRoots()
    {
        var roots = _classesAndDefs.Keys.ToList();
        foreach (var root in Relations)
        {
            foreach (var childName in root.Value)
                roots.Remove(childName);
        }

        return roots;
    }

    public Prim GetPrim(string name)
    {
        return _classesAndDefs[name];
    }

    public Overs GetOvers()
        => new Overs(Prims.OfType<Over>());

    public IEnumerable<Prim> GetNeighbours(Prim prim)
    {
        if (!Relations.TryGetValue(prim.Name, out ChildNames? value))
            return [];

        var children = new List<Prim>();
        foreach (var childName in value)
        {
            if (_classesAndDefs.TryGetValue(childName, out Prim? foundPrim))
                children.Add(foundPrim);
        }

        return children;
    }
}

public class Overs
{
    private readonly Dictionary<string, List<Over>> _overs;

    public Overs(IEnumerable<Over> overs)
    {
        _overs = overs.GroupBy(o => o.Name).ToDictionary(g => g.Key, g => g.ToList());
    }

    public IEnumerable<ComponentJson> GetComponentsFor(string name)
    {
        if (_overs.TryGetValue(name, out List<Over>? value))
            return value.Select(o => o.Component).OfType<ComponentJson>();
        else
            return [];
    }
}

public class ChildNames : List<string>
{

}