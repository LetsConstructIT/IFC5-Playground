using IFC5.Reader.Domain;
using IFC5.Reader.Models;
using IFC5.Reader.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IFC5.Reader.Composers;
internal class DefsComposer
{
    private readonly RootPrims _rootPrims;
    private readonly Overs _overs;

    public DefsComposer(RootPrims rootPrims, Overs overs)
    {
        _rootPrims = rootPrims;
        _overs = overs;
    }

    internal ComposedObjects Compose()
    {
        var composed = new ComposedObjects();

        foreach (var rootPrim in _rootPrims)
        {
            composed.Add(Compose(rootPrim));
        }

        return composed;
    }

    private ComposedObject Compose(Prim prim)
    {
        var type = string.Empty;
        var components = new List<ComponentJson>();

        if (prim is Def def)
        {
            type = def.Type;
            if (def.Component is not null)
                components.Add(def.Component);
        }

        var composedDef = new ComposedObject(prim.Name, type, components);
        composedDef.Components.AddRange(_overs.GetComponentsFor(composedDef.Name));

        foreach (var child in prim.Children)
        {
            var composedChild = Compose(child);

            if (child is Def)
            {
                composedDef.Children.Add(composedChild);
            }
            else if (child is Class) //flattening classes
            {
                composedDef.Children.AddRange(composedChild.Children);
                composedDef.Components.AddRange(composedChild.Components);
            }
        }

        return composedDef;
    }
}

[DebuggerDisplay("{Name}")]
public class ComposedObject
{
    public string Name { get; }
    public string Type { get; }
    public List<ComponentJson> Components { get; }
    public List<ComposedObject> Children { get; }

    public ComposedObject(string name, string type, List<ComponentJson> components)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Components = components ?? throw new ArgumentNullException(nameof(components));
        Children = new List<ComposedObject>();
    }

    public string GetFriendlyName()
    {
        if (Name.Contains("__"))
            return Name.Substring(Name.IndexOf("__") + 2);
        else
            return Name;
    }
}

public class ComposedObjects : List<ComposedObject>
{
    public void DummyPrint()
    {
        var level = 0;

        foreach (var prim in this)
            DummyPrint(prim, level);
    }

    private void DummyPrint(ComposedObject prim, int level)
    {
        Console.WriteLine($"{new string(' ', level)}{prim.GetFriendlyName()}");

        foreach (var item in prim.Children)
        {
            DummyPrint(item, level + 1);
        }
    }
}