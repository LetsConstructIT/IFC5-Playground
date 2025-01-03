using IFC5.Reader.Models.DTOs;
using System.Collections.Generic;

namespace IFC5.Reader.Composers;
internal class Composer
{
    public ComposedObjects Compose(IEnumerable<PrimJson> prims)
    {
        // let's bring out children and inherits
        var flattenedTree = new TreeFlattener(prims).Flatten();

        // graph composition with help of DFS
        var rootPrims = new TreeComposer(flattenedTree).Compose();

        // collapsing classes and attaching overs to defs
        var composedObjects = new DefsComposer(rootPrims, flattenedTree.GetOvers()).Compose();

        return composedObjects;
    }
}
