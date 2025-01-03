using Rhino.Geometry;
using Rhino;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using IFC5.Reader.Composers;
using IFC5.Reader.Models;
using System;
using Rhino.DocObjects;

namespace IFC5.RhinoImport;
internal class Ifc5Inserter
{
    private readonly MeshCreator _meshCreator = new();
    private readonly List<Ifc5Material> _materials = new();

    internal void Insert(RhinoDoc doc, ComposedObjects composedObjects)
    {
        var rawMaterials = composedObjects.Where(c => c.Type == "UsdShade:Material");
        PopulateMaterials(rawMaterials);

        foreach (var composedObject in composedObjects.Except(rawMaterials))
        {
            var transformation = Transform.Identity;
            var material = Color.White;
            var name = "";
            InsertObject(doc, composedObject, transformation, material, name);
        }
    }

    private void PopulateMaterials(IEnumerable<ComposedObject> rawMaterials)
    {
        _materials.Clear();
        foreach (var materialObject in rawMaterials)
        {
            foreach (var potentialShader in materialObject.Children)
            {
                var shaders = potentialShader.Components.OfType<UsdShadeShaderComponent>();
                if (!shaders.Any())
                    continue;

                _materials.Add(new Ifc5Material(materialObject.Name, shaders.Last().ToRhino()));
                break;
            }
        }
    }

    private void InsertObject(RhinoDoc doc, ComposedObject composedObject, Transform transformation, Color material, string name)
    {
        name = $"{name}/{composedObject.GetFriendlyName()}";
        transformation = AdjustTransformation(composedObject.Components, transformation);
        material = AdjustMaterial(composedObject.Components, material);

        var layerIndex = CreateOrReuseLayer(doc, name);

        if (HasVisibleMesh(composedObject.Components, out UsdGeomMeshComponent? usdGeomMesh) && usdGeomMesh is not null)
        {
            var mesh = _meshCreator.CreateRhinoMesh(usdGeomMesh);
            mesh.Transform(transformation);
            mesh.VertexColors.CreateMonotoneMesh(material);

            var attributes = ComposeAttributes(layerIndex, name, composedObject.Components);

            doc.Objects.Add(mesh, attributes);
        }

        foreach (var child in composedObject.Children)
        {
            InsertObject(doc, child, transformation, material, name);
        }
    }

    private static ObjectAttributes ComposeAttributes(int layerIndex, string name, List<ComponentJson> components)
    {
        var attributes = new ObjectAttributes();
        if (layerIndex != -1)
            attributes.LayerIndex = layerIndex;

        attributes.SetUserString("Name", name);

        foreach (var component in components)
        {
            if (!component.TryConvertToAttributes(out Dictionary<string, string>? toDisplay) || toDisplay is null)
                continue;

            foreach (var keyValuePair in toDisplay)
                attributes.SetUserString(keyValuePair.Key, keyValuePair.Value);
        }

        return attributes;
    }

    private int CreateOrReuseLayer(RhinoDoc doc, string name)
    {
        var existingLayer = doc.Layers.FindName(name);

        if (existingLayer != null)
            return existingLayer.Index;
        else
            return doc.Layers.Add(new Layer() { Name = name });
    }

    private Color AdjustMaterial(List<ComponentJson> components, Color material)
    {
        var materialBindings = components.OfType<UsdShadeMaterialBindingApiComponent>();
        if (!materialBindings.Any())
            return material;

        var materialName = SanitizeName(materialBindings.Last());
        var newMaterial = _materials.FirstOrDefault(c => c.Name == materialName);

        return newMaterial switch
        {
            not null => newMaterial.Color,
            _ => material
        };

        string SanitizeName(UsdShadeMaterialBindingApiComponent usdShadeMaterialBindingApiComponent)
        {
            var rawName = usdShadeMaterialBindingApiComponent.MaterialBinding!.Ref!;
            // </WallMaterial>
            return rawName.Substring(2, rawName.Length - 3);
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

public class Ifc5Material
{
    public string Name { get; }
    public Color Color { get; }

    public Ifc5Material(string name, Color color)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Color = color;
    }
}