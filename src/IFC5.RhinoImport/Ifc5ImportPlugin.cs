using IFC5.Reader.Composers;
using IFC5.Reader.Models;
using Rhino;
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

        foreach (var composedObject in composedObjects)
        {
            AddMesh(doc, composedObject);
        }

        return true;
    }

    private void AddMesh(RhinoDoc doc, ComposedObject composedObject)
    {
        var usdGeomMeshes = composedObject.Components.OfType<UsdGeomMeshComponent>().ToList();
        if (usdGeomMeshes.Any() && ShouldBeVisible(composedObject))
        {
            var usdGeomMesh = usdGeomMeshes.Last();
            var mesh = _meshCreator.CreateRhinoMesh(usdGeomMesh);
            doc.Objects.Add(mesh);
        }

        foreach (var child in composedObject.Children)
        {
            AddMesh(doc, child);
        }
    }

    private bool ShouldBeVisible(ComposedObject composedObject)
    {
        var visibility = composedObject.Components.OfType<UsdGeomVisibilityApiVisibilityComponent>().ToList();
        if (!visibility.Any())
            return true;

        return visibility.Last().Visibility != "invisible";
    }
}