using IFC5Tekla.Engine.Models;
using System.Collections.Generic;

namespace IFC5Tekla.Engine;
internal class Ifc5Composer
{
    public void Compose(IEnumerable<PrimJson> prims)
    {
        var flattenedTree = new TreeFlattener(prims).Flatten();

        var rootPrims = new TreeComposer(flattenedTree).Compose();
        rootPrims.DummyPrint();

    }
}
