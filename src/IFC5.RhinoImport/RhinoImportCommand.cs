using Rhino;
using Rhino.Commands;

namespace IFC5.RhinoImport;
public class RhinoImportCommand : Command
{
    public RhinoImportCommand()
    {
        Instance = this;
    }

    public static RhinoImportCommand Instance { get; private set; }

    public override string EnglishName => "IFC5ImportCommand";

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
        return Result.Success;
    }
}
