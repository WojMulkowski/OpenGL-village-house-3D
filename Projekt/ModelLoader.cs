using Assimp;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Mathematics;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Formats.Asn1.AsnWriter;
using System.Xml.Linq;

// WCZYTUJE PLIKI Z ROZSZERZENIEM OBJ
public class ModelLoader
{
    public List<Vector3> Vertices { get; private set; }
    public List<Vector3> Normals { get; private set; }
    public List<Vector2> TexCoords { get; private set; }
    public int VertexCount { get; private set; }

    public ModelLoader(string filePath)
    {
        Vertices = new List<Vector3>();
        Normals = new List<Vector3>();
        TexCoords = new List<Vector2>();

        LoadModel(filePath);
    }

    private void LoadModel(string filePath)
    {
        AssimpContext importer = new AssimpContext();
        Scene scene = importer.ImportFile(filePath, PostProcessPreset.TargetRealTimeQuality);

        if (scene == null || scene.RootNode == null)
        {
            throw new System.Exception("Error loading model.");
        }

        ProcessNode(scene.RootNode, scene);
    }

    private void ProcessNode(Node node, Scene scene)
    {
        foreach (int meshIndex in node.MeshIndices)
        {
            ProcessMesh(scene.Meshes[meshIndex], scene);
        }

        foreach (Node childNode in node.Children)
        {
            ProcessNode(childNode, scene);
        }
    }

    private void ProcessMesh(Mesh mesh, Scene scene)
    {
        for (int i = 0; i < mesh.VertexCount; i++)
        {
            Vector3D vertex = mesh.Vertices[i];
            Vertices.Add(new Vector3(vertex.X, vertex.Y, vertex.Z));

            Vector3D normal = mesh.Normals[i];
            Normals.Add(new Vector3(normal.X, normal.Y, normal.Z));

            if (mesh.HasTextureCoords(0))
            {
                Vector3D texCoord = mesh.TextureCoordinateChannels[0][i];
                TexCoords.Add(new Vector2(texCoord.X, texCoord.Y));
            }
        }

        VertexCount = mesh.VertexCount;
    }
}
