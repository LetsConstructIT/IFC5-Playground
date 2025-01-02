using IFC5Tekla.Engine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IFC5Tekla.Engine.Domain;

[DebuggerDisplay("{Name}")]
public class Prim
{
    public string Name { get; }
    public List<Prim> Children { get; }

    public Prim(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Children = new List<Prim>();
    }

    public string CleanInheritName(string input)
    {
        var offset = 2; // first two chars are </
        return input.Substring(offset, input.Length - offset - 1);
    }

    public override int GetHashCode() => Name.GetHashCode();
    public override bool Equals(object obj)
    {
        if (obj is not Prim prim) return false;

        return Name.Equals(prim.Name);
    }
}

public class Class : Prim
{
    public string Type { get; }
    public string[] Inherits { get; }

    public Class(string name, string[] inherits, string type) : base(name)
    {
        Inherits = inherits.Select(CleanInheritName).ToArray();
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }
}

public class Def : Prim
{
    public string Type { get; }
    public ComponentJson Component { get; }
    public string[] Inherits { get; }

    public Def(string name, string[] inherits, string type, ComponentJson component) : base(name)
    {
        Inherits = inherits.Select(CleanInheritName).ToArray();
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Component = component ?? throw new ArgumentNullException(nameof(component));
    }
}

public class Over : Prim
{
    public ComponentJson Component { get; }

    public Over(string name, ComponentJson component) : base(name)
    {
        Component = component ?? throw new ArgumentNullException(nameof(component));
    }
}