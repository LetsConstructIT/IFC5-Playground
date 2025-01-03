using IFC5.Reader.Models.DTOs;
using Rhino.Geometry;
using System.Drawing;

namespace IFC5.RhinoImport;
internal static class RhinoMappers
{
    public static Transform ToRhino(this XformOpComponent component)
    {
        var transform = new Transform();
        for (int i = 0; i < component.Transform!.Length; i++)
        {
            var row = component.Transform[i];
            for (int j = 0; j < row.Length; j++)
                transform[i, j] = row[j];
        }

        return transform.Transpose();
    }

    public static Color ToRhino(this UsdShadeShaderComponent usdShadeShaderComponent)
    {
        var rawColor = usdShadeShaderComponent.DiffuseColor!;

        return Color.FromArgb(To255(usdShadeShaderComponent.Opacity!.Value),
                              To255(rawColor[0]),
                              To255(rawColor[1]),
                              To255(rawColor[2]));

        int To255(double value)
            => (int)(value * 255);
    }
}
