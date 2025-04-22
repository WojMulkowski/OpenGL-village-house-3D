using System;
using System.IO;
using ObjLoader.Loader.Loaders;
using System.Collections.Generic;

public class Model
{
    public int vertexCount;
    public List<float> vertices = new();
    public List<float> normals = new();
    public List<float> vertexNormals = new();
    public List<float> texCoords = new();
}

public class ObjModelLoader
{
    private readonly ObjLoaderFactory _objLoaderFactory = new();

    public void Load(string filePath, Model model)
    {
        // Uzyskaj ścieżkę do katalogu aplikacji
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        // Dodaj względną ścieżkę do katalogu "Obiekty"
        string fullPath = Path.Combine(basePath, "Obiekty", filePath);

        // Utwórz objLoader i dostosuj MaterialStreamProvider
        var objLoader = _objLoaderFactory.Create(new CustomMaterialStreamProvider(basePath));

        using (var fileStream = new FileStream(fullPath, FileMode.Open))
        {
            var result = objLoader.Load(fileStream);

            int vertexCount = 0;

            foreach (var group in result.Groups)
            {
                foreach (var face in group.Faces)
                {
                    for (int i = 0; i < face.Count; i++)
                    {
                        var faceVertex = face[i];
                        var vertex = result.Vertices[faceVertex.VertexIndex - 1];
                        model.vertices.Add(vertex.X);
                        model.vertices.Add(vertex.Y);
                        model.vertices.Add(vertex.Z);
                        model.vertices.Add(1);

                        if (faceVertex.TextureIndex > 0)
                        {
                            var texture = result.Textures[faceVertex.TextureIndex - 1];
                            model.texCoords.Add(texture.X);
                            model.texCoords.Add(texture.Y);
                        }

                        if (faceVertex.NormalIndex > 0)
                        {
                            var normal = result.Normals[faceVertex.NormalIndex - 1];
                            model.vertexNormals.Add(normal.X);
                            model.vertexNormals.Add(normal.Y);
                            model.vertexNormals.Add(normal.Z);
                            model.vertexNormals.Add(0);
                        }
                    }
                }

                vertexCount += group.Faces.Count * group.Faces[0].Count;
            }

            model.vertexCount = vertexCount;
        }
    }

    // Implementacja dostawcy strumienia materiałów w tej samej klasie
    private class CustomMaterialStreamProvider : IMaterialStreamProvider
    {
        private readonly string _basePath;

        public CustomMaterialStreamProvider(string basePath)
        {
            // Ustaw bazową ścieżkę na poziom wyżej, aby uzyskać dostęp do katalogu projektu
            _basePath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\"));
        }

        public Stream Open(string materialFilePath)
        {
            string fullPath = Path.Combine(_basePath, "Obiekty", materialFilePath);
            return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        }
    }
}
