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

    internal ComposedObjects Compose()
    {
        var composed = new ComposedObjects();

        foreach (var rootPrim in _rootPrims)
        {
            if (rootPrim is Def def)
            {
                var composedDef = new ComposedDef(def.Name, def.Type, [def.Component]);

                composed.Add(composedDef);
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