using Eto.Forms.ThemedControls;
using IFC5.Reader.Models;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFC5.RhinoImport;
internal class MeshCreator
{

    public Mesh CreateRhinoMesh(UsdGeomMeshComponent usdGeomMesh)
    {
        var mesh = new Mesh();
        foreach (var coords in usdGeomMesh.Points)
        {
            mesh.Vertices.Add(coords[0], coords[1], coords[2]);
        }

        var indicies = usdGeomMesh.FaceVertexIndices;
        var countPerFace = usdGeomMesh.GetFaceVertexCount();
        for (int i = 0; i < indicies.Length; i += countPerFace)
        {
            mesh.Faces.AddFace(indicies[i], indicies[i + 1], indicies[i + 2]);
        }

        mesh.Normals.ComputeNormals();

        return mesh;
    }
}
