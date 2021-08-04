using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;

namespace _3DGraphicsLib.OpenGL.Objects
{
    class Model
    {
        public List<Mesh> meshes = new List<Mesh>();

        public Model(string path)
        {
            AssimpContext importer = new AssimpContext();
            var scene = importer.ImportFile(path, PostProcessSteps.Triangulate | PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.FlipUVs | PostProcessSteps.CalculateTangentSpace);
            ProcessNode(scene.RootNode, scene);
        }

        public void Draw(Shader shader)
        {
            if (meshes == null) return;
            foreach (var mesh in meshes)
            {
                mesh.Draw(shader);
            }
        }

        private void ProcessNode(Node node, Scene scene)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                var mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, scene));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            List<float> vertexData = new List<float>();
            List<int> indices = new List<int>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                vertexData.Add(mesh.Vertices[i].X);
                vertexData.Add(mesh.Vertices[i].Y);
                vertexData.Add(mesh.Vertices[i].Z);

                if (mesh.HasNormals)
                {
                    vertexData.Add(mesh.Normals[i].X);
                    vertexData.Add(mesh.Normals[i].Y);
                    vertexData.Add(mesh.Normals[i].Z);
                }

                // temp texture coords
                vertexData.Add(0f);
                vertexData.Add(0f);
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                var face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add(face.Indices[j]);
                }
            }

            return new Mesh(vertexData.ToArray(), indices.ToArray());
        }
    }
}
