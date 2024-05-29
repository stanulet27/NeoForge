using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    [Serializable]
    public class MeshData
    {
        public float[] Vertices;
        public int[] Triangles;
        public float[] Hits;
        public float Force;
        public float[] Direction;
        
        /// <summary>
        /// Creates the data structure that is passed between the game and the Python server.
        /// </summary>
        /// <param name="mesh">The surface mesh being modified</param>
        /// <param name="hits">The vertices that are going to be impacted</param>
        /// <param name="force">The amount of force being applied by the hit</param>
        /// <param name="direction">The direction that the hit is coming from</param>
        public MeshData(Mesh mesh, IEnumerable<Vector3> hits, float force, Vector3 direction)
        {
            Hits = Unwrap(hits);
            Force = force;
            Direction = new[] {direction.x, direction.y, direction.z};
            
            var meshVertices = mesh.vertices;
            Vertices = Unwrap(meshVertices);
            Triangles = mesh.triangles;
            
            Debug.Assert(Vertices is { Length: > 0 }, "Mesh vertices are not properly assigned or empty.");
            Debug.Assert(Triangles is { Length: > 0 }, "Mesh triangles are not properly assigned or empty.");
        }

        private float[] Unwrap(IEnumerable<Vector3> points)
        {
            return points.SelectMany(p => new[] { p.x, p.y, p.z }).ToArray();;
        }
    }
}