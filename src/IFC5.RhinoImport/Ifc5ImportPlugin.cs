using Rhino;
using System;

namespace IFC5.RhinoImport;

public class Ifc5ImportPlugin : Rhino.PlugIns.FileImportPlugIn
{
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
        bool read_success = false;

        return read_success;
    }
}