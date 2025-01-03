using IFC5.Reader.Models.DTOs;
using System.Collections.Generic;

namespace IFC5.Reader.Composers;
internal class Composer
{
    public ComposedObjects Compose(IEnumerable<PrimJson> prims)
    {
        var flattenedTree = new TreeFlattener(prims).Flatten();

        var rootPrims = new TreeComposer(flattenedTree).Compose();

        var composedObjects = new DefsComposer(rootPrims, flattenedTree.GetOvers()).Compose();

        return composedObjects;
    }
}
