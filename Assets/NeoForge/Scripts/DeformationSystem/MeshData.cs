using System;
using UnityEngine;

namespace DeformationSystem
{
    [Serializable]
    public class MeshData
    {
        public float[] vertices;
        public int[] triangles;
        public float[] hits;
        public float force;
        public float[] direction;
        
        public MeshData(Mesh mesh, Vector3[] hits, float force, Vector3 direction)
        {
            this.hits = Unwrap(hits);
            this.force = force;
            this.direction = new[] {direction.x, direction.y, direction.z};
            var meshVertices = mesh.vertices;
            vertices = Unwrap(meshVertices);

            triangles = mesh.triangles;

            if (vertices == null || vertices.Length == 0)
            {
                Debug.LogError("Mesh vertices are not properly assigned or empty.");
            }

            if (triangles == null || triangles.Length == 0)
            {
                Debug.LogError("Mesh triangles are not properly assigned or empty.");
            }
        }

        private float[] Unwrap(Vector3[] points)
        {
            var unwrapped = new float[points.Length * 3];
            for (int i = 0; i < points.Length; i++)
            {
                unwrapped[i * 3] = points[i].x;
                unwrapped[i * 3 + 1] = points[i].y;
                unwrapped[i * 3 + 2] = points[i].z;
            }

            return unwrapped;
        }
    }
}