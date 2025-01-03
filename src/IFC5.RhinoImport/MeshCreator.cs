using IFC5.Reader.Models;
using Rhino.Geometry;

namespace IFC5.RhinoImport;
internal class MeshCreator
{

    public Mesh CreateRhinoMesh(UsdGeomMeshComponent usdGeomMesh)
    {
        var mesh = new Mesh();
        foreach (var coords in usdGeomMesh.Points!)
        {
            mesh.Vertices.Add(coords[0], coords[1], coords[2]);
        }

        var indicies = usdGeomMesh.FaceVertexIndices!;
        var countPerFace = GetFaceVertexCount(usdGeomMesh);
        for (int i = 0; i < indicies.Length; i += countPerFace)
        {
            mesh.Faces.AddFace(indicies[i], indicies[i + 1], indicies[i + 2]);
        }

        mesh.Normals.ComputeNormals();

        return mesh;
    }

    private int GetFaceVertexCount(UsdGeomMeshComponent usdGeomMesh)
    {
        if (usdGeomMesh.FaceVertexCounts == null)
            return 3;

        return usdGeomMesh.FaceVertexCounts[0];
    }
}
