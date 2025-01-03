using IFC5.Reader.Composers;
using IFC5.Reader.Models;
using Rhino;
using Rhino.Geometry;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IFC5.RhinoImport;

public class Ifc5ImportPlugin : Rhino.PlugIns.FileImportPlugIn
{
    private readonly MeshCreator _meshCreator = new();

    public Ifc5ImportPlugin()
    {
        Instance = this;
    }

    public static Ifc5ImportPlugin Instance { get; private set; }

    protected override Rhino.PlugIns.FileTypeList AddFileTypes(Rhino.FileIO.FileReadOptions options)
    {
        var result = new Rhino.PlugIns.FileTypeList();
        result.AddFileType("IFC5 file (*.ifcx)", "ifcx");
        return result;
    }

    protected override bool ReadFile(string filename, int index, RhinoDoc doc, Rhino.FileIO.FileReadOptions options)
    {
        var composedObjects = new Reader.Reader().Read(filename);

        var transformation = Transform.Identity;
        foreach (var composedObject in composedObjects)
        {
            AddMesh(doc, composedObject, transformation);
        }

        return true;
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