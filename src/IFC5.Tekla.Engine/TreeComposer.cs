using IFC5Tekla.Engine.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC5Tekla.Engine;
internal class TreeComposer
{
    private readonly FlattenedTree _inputTree;

    public TreeComposer(FlattenedTree inputTree)
    {
        _inputTree = inputTree;
    }

    public void Compose()
    {
        var roots = FindRoots();

    }

    private HashSet<string> FindRoots()
    {
        var roots = _inputTree.Prims.Where(p => p is Class || p is Def).Select(p => p.Name).ToHashSet();
        foreach (var root in _inputTree.Relations)
        {
            foreach (var childName in root.Value)
                roots.Remove(childName);
        }

        return roots;
    }
}
