using Rhino.Geometry;
using Rhino;
using System.Collections.Generic;
using System.Linq;
using IFC5.Reader.Composers;
using IFC5.Reader.Models;

namespace IFC5.RhinoImport;
internal class Ifc5Inserter
{
    private readonly MeshCreator _meshCreator = new();

    internal void Insert(RhinoDoc doc, ComposedObjects composedObjects)
    {
        var transformation = Transform.Identity;
        foreach (var composedObject in composedObjects)
        {
            AddMesh(doc, composedObject, transformation);
        }
    }

    private void AddMesh(RhinoDoc doc, ComposedObject composedObject, Transform transformation)
    {
        transformation = AdjustTransformation(composedObject.Components, transformation);

        if (HasVisibleMesh(composedObject.Components, out UsdGeomMeshComponent? usdGeomMesh) && usdGeomMesh is not null)
        {
            var mesh = _meshCreator.CreateRhinoMesh(usdGeomMesh);
            mesh.Transform(transformation);
            doc.Objects.Add(mesh);
        }

        foreach (var child in composedObject.Children)
        {
            AddMesh(doc, child, transformation);
        }
    }

    private Transform AdjustTransformation(List<ComponentJson> components, Transform transformation)
    {
        var xForms = components.OfType<XformOpComponent>();
        if (!xForms.Any())
            return transformation;

        var transformationToAdd = xForms.Last().ToRhino();
        return transformation * transformationToAdd;
    }

    private bool HasVisibleMesh(List<ComponentJson> components, out UsdGeomMeshComponent? usdGeomMesh)
    {
        usdGeomMesh = null;

        var usdGeomMeshes = components.OfType<UsdGeomMeshComponent>();
        var visibilities = components.OfType<UsdGeomVisibilityApiVisibilityComponent>();

        if (usdGeomMeshes.Any() && ShouldBeVisible(visibilities))
        {
            usdGeomMesh = usdGeomMeshes.Last();
            return true;
        }

        return false;
    }

    private bool ShouldBeVisible(IEnumerable<UsdGeomVisibilityApiVisibilityComponent> visibilities)
    {
        if (!visibilities.Any())
            return true;

        return visibilities.Last().Visibility != "invisible";
    }
}
