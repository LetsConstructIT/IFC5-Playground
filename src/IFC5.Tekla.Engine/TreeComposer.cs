using IFC5Tekla.Engine.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tekla.Common.Geometry.Shapes.Zkit.TriangleMesh;
using Tekla.Structures.TeklaStructuresInternal.MateriaClient;
using System.Xml.Linq;

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

        var graph = new Graph(_inputTree.Relations);
        foreach (var rootName in roots)
        {
            var rootNode = new Node(rootName);
            var solution = DepthFirstTraversal(graph, rootName).ToList();
        }


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

    public IEnumerable<string> DepthFirstTraversal(
                        IGraph graph,
                        string start)
    {
        var visited = new HashSet<string>();
        var stack = new Stack<string>();

        stack.Push(start);

        while (stack.Count != 0)
        {
            var current = stack.Pop();

            if (!visited.Add(current))
                continue;

            yield return current;

            var neighbours = graph.GetNeighbours(current)
                                  .Where(n => !visited.Contains(n));

            foreach (var neighbour in neighbours.Reverse())
                stack.Push(neighbour);
        }
    }

    public class Node
    {
        public string Name { get; }
        public List<Node> Children { get; }

        public Node(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Children = new List<Node>();
        }
    }

    public interface IGraph
    {
        IEnumerable<string> GetNeighbours(string name);
    }

    public class Graph : IGraph
    {
        private Dictionary<string, ChildNames> _relations;

        public Graph(Dictionary<string, ChildNames> relations)
        {
            _relations = relations ?? throw new ArgumentNullException(nameof(relations));
        }

        public IEnumerable<string> GetNeighbours(string key)
        {
            if (_relations.ContainsKey(key))
                return _relations[key];
            else
                return Enumerable.Empty<string>();
        }
    }

}
