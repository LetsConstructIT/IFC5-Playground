using IFC5Tekla.Engine.Domain;
using System.Collections.Generic;

namespace IFC5Tekla.Engine;
internal class TreeComposer
{
    private readonly FlattenedTree _inputTree;

    public TreeComposer(FlattenedTree inputTree)
    {
        _inputTree = inputTree;
    }

    public RootPrims Compose()
    {
        var rootPrims = new RootPrims();

        foreach (var rootName in _inputTree.FindRoots())
        {
            var rootPrim = _inputTree.GetPrim(rootName);
            DepthFirstTraversal(_inputTree, rootPrim);

            rootPrims.Add(rootPrim);
        }

        return rootPrims;
    }

    public Prim DepthFirstTraversal(IPrimGraph graph, Prim start)
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

public class RootPrims : List<Prim>
{

}