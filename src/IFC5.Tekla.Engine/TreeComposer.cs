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
        var roots = _inputTree.FindRoots();

        foreach (var rootName in roots)
        {
            var rootPrim = _inputTree.GetPrim(rootName);
            DepthFirstTraversal(_inputTree, rootPrim);

        }
    }

    public Prim DepthFirstTraversal(
                    IPrimGraph graph,
                    Prim start)
    {
        var visited = new HashSet<Prim>();
        var stack = new Stack<Prim>();

        stack.Push(start);

        while (stack.Count != 0)
        {
            var current = stack.Pop();

            if (!visited.Add(current))
                continue;

            foreach (var child in graph.GetNeighbours(current))
            {
                if (!visited.Contains(child))
                stack.Push(child);

                current.Children.Add(child);
            }
        }

        return start;
    }
}
