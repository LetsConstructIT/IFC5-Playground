using IFC5Tekla.Engine.Domain;
using IFC5Tekla.Engine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC5Tekla.Engine;
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
            if (rootPrim is not Def def)
                continue;

            composed.Add(Compose(def));
        }

        return composed;
    }

    private ComposedDef Compose(Def def)
    {
        var composedDef = new ComposedDef(def.Name, def.Type, [def.Component]);
        composedDef.Components.AddRange(_overs.GetComponentsFor(composedDef.Name));

        foreach (var child in def.Children)
        {
            if (child is Class innerClass)
            {
                composedDef.Components.AddRange(_overs.GetComponentsFor(innerClass.Name));

                foreach (var innerChild in innerClass.Children.OfType<Def>())
                    composedDef.Children.Add(Compose(innerChild));

                // class inheriting class ??
                var subclasses = innerClass.Children.OfType<Class>().ToList();
                if (subclasses.Any())
                {
                    var test = subclasses.First();
                }
            }
            else if (child is Def innerDef)
            {
                composedDef.Children.Add(Compose(innerDef));
            }
        }
        return composedDef;
    }
}

[DebuggerDisplay("{Name}")]
public class ComposedDef
{
    public string Name { get; }
    public string Type { get; }
    public List<ComponentJson> Components { get; }
    public List<ComposedDef> Children { get; }

    public ComposedDef(string name, string type, List<ComponentJson> components)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Components = components ?? throw new ArgumentNullException(nameof(components));
        Children = new List<ComposedDef>();
    }
}

public class ComposedObjects : List<ComposedDef>
{

}