using System;
using UnityEngine;

namespace NeoForge.Deformation.JSON
{
    [Serializable]
    public class MeshData
    {
        public float[] Vertices;
        public int[] Triangles;
  
        public MeshData(Mesh mesh, float scale = 1f)
        {
            var meshVertices = mesh.vertices ;
            Vertices = Unwrap(meshVertices, scale);

            Triangles = mesh.triangles;

            if (Vertices == null || Vertices.Length == 0)
            {
                Debug.LogError("Mesh vertices are not properly assigned or empty.");
            }

            if (Triangles == null || Triangles.Length == 0)
            {
                Debug.LogError("Mesh triangles are not properly assigned or empty.");
            }
        }

        private float[] Unwrap(Vector3[] points, float scale)
        {
            var unwrapped = new float[points.Length * 3];
            for (int i = 0; i < points.Length; i++)
            {
                unwrapped[i * 3] = points[i].x * scale;
                unwrapped[i * 3 + 1] = points[i].y * scale;
                unwrapped[i * 3 + 2] = points[i].z * scale;
            }

            return unwrapped;
        }
    }
}