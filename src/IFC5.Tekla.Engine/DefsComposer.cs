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

    public DefsComposer(RootPrims rootPrims)
    {
        _rootPrims = rootPrims;
    }

    internal ComposedObjects Compose(Overs overs)
    {
        var composed = new ComposedObjects();

        foreach (var rootPrim in _rootPrims)
        {
            if (rootPrim is not Def def)
                continue;

            var composedDef = new ComposedDef(def.Name, def.Type, [def.Component]);
            composedDef.Components.AddRange(overs.GetComponentsFor(composedDef.Name));

            composed.Add(composedDef);

            foreach (var child in def.Children)
            {
                if (child is Class innerClass)
                {
                    composedDef.Components.AddRange(overs.GetComponentsFor(innerClass.Name));
                }
                else if (child is Def innerDef)
                {

                }
            }
        }

        return composed;
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