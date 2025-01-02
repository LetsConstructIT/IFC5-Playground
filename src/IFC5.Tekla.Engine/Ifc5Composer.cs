using IFC5.Reader.Models;
using System.Collections.Generic;

namespace IFC5.Reader;
internal class Ifc5Composer
{
    public void Compose(IEnumerable<PrimJson> prims)
    {
        var flattenedTree = new TreeFlattener(prims).Flatten();

        var rootPrims = new TreeComposer(flattenedTree).Compose();

        var composedObjects = new DefsComposer(rootPrims, flattenedTree.GetOvers()).Compose();
        composedObjects.DummyPrint();
    }
}
