using System;
using System.Collections.Generic;
using UnityEngine;

namespace DeformationSystem
{
    [Serializable]
    public class ResultData
    {
        public float[] Vertices;
        public int[] Triangles;
        public int Steps;
        
        public ResultData(float[] verticies, int[] traingles, int steps)
        {
            Vertices = verticies;
            Triangles = traingles;
            Steps = steps;
        }

        private Mesh CreateMesh(float[] verticies)
        {
            var mesh = new Mesh();
            var meshVertices = new Vector3[verticies.Length / 3];
            for (var i = 0; i < verticies.Length; i += 3)
            {
                meshVertices[i / 3] = new Vector3(verticies[i], verticies[i + 1], verticies[i + 2]);
            }
            mesh.vertices = meshVertices;
            mesh.triangles = Triangles;
            mesh.RecalculateNormals();
            mesh.Optimize();
            return mesh;
        }
        
        public List<Mesh> ExtractMeshes()
        {
            int singleArrayLength = Vertices.Length / Steps;
            var vertexLists = new List<float[]>();
            for (int i = 0; i < Vertices.Length; i += singleArrayLength)
            {
                var singleArray = new float[singleArrayLength];
                Array.Copy(Vertices, i, singleArray, 0, singleArrayLength);
                vertexLists.Add(singleArray);
            }
            var Meshes = new List<Mesh>();
            foreach (var vertexList in vertexLists)
            {
                Meshes.Add(CreateMesh(vertexList));
            }
            return Meshes;
        }
    }
}