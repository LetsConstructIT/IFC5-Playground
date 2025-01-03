using Rhino.Geometry;
using Rhino;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using IFC5.Reader.Composers;
using IFC5.Reader.Models;
using System;
using Eto.Forms;

namespace IFC5.RhinoImport;
internal class Ifc5Inserter
{
    private readonly MeshCreator _meshCreator = new();

    internal void Insert(RhinoDoc doc, ComposedObjects composedObjects)
    {
        var transformation = Transform.Identity;
        var material = Color.White;

        foreach (var composedObject in composedObjects)
        {
            AddMesh(doc, composedObject, transformation, material);
        }
    }

    private void AddMesh(RhinoDoc doc, ComposedObject composedObject, Transform transformation, Color material)
    {
        transformation = AdjustTransformation(composedObject.Components, transformation);
        material = AdjustMaterial(composedObject.Components, material);

        if (HasVisibleMesh(composedObject.Components, out UsdGeomMeshComponent? usdGeomMesh) && usdGeomMesh is not null)
        {
            var mesh = _meshCreator.CreateRhinoMesh(usdGeomMesh);
            mesh.Transform(transformation);
            mesh.VertexColors.CreateMonotoneMesh(System.Drawing.Color.FromArgb(128, 0, 128, 0));
            doc.Objects.Add(mesh);
        }

        foreach (var child in composedObject.Children)
        {
            AddMesh(doc, child, transformation, material);
        }
    }

    private Color AdjustMaterial(List<ComponentJson> components, Color material)
    {
        var materialBindings = components.OfType<UsdShadeMaterialBindingApiComponent>();
        if (!materialBindings.Any())
            return material;

        var materialBinding = materialBindings.Last();

        return material;
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
