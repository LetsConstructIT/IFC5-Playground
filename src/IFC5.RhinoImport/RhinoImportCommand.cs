using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace IFC5.RhinoImport;
public class RhinoImportCommand : Command
{
    public RhinoImportCommand()
    {
        // Rhino only creates one instance of each command class defined in a
        // plug-in, so it is safe to store a refence in a static property.
        Instance = this;
    }

    ///<summary>The only instance of this command.</summary>
    public static RhinoImportCommand Instance { get; private set; }

    ///<returns>The command name as it appears on the Rhino command line.</returns>
    public override string EnglishName => "IFC5ImportCommand";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
        // Usually commands in import plug-ins are used to modify settings and behavior.
        // The import work itself is performed by the Ifc5ImportPlugin class.
        return Result.Success;
    }
}
