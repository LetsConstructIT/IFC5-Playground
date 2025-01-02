using IFC5.Reader.Models;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC5.RhinoImport;
internal static class RhinoMappers
{
    public static Transform ToRhino(this XformOpComponent component)
    {
        var transform = new Transform();
        for (int i = 0; i < component.Transform.Length; i++)
        {
            var row = component.Transform[i];
            for (int j = 0; j < row.Length; j++)
                transform[i, j] = row[j];
        }

        return transform.Transpose();
    }
}
