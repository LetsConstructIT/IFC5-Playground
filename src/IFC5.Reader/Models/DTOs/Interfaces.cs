namespace IFC5.Reader.Models.DTOs;

public interface IParent
{
    public DefJson[]? Children { get; set; }
}

public interface IComponentable
{
    public ComponentJson? Component { get; set; }
}
