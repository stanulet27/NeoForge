using System;
using AYellowpaper.SerializedCollections;
using NeoForge.Utilities;
using UnityEngine;

namespace NeoForge.Deformation.JSON
{
    public class PartMeshDatabase : SingletonMonoBehaviour<PartMeshDatabase>
    {
        public enum Shape { Rectangular, Spherical }
        public enum Size { Small, Medium, Large }
        
        [SerializeField] private SerializedDictionary<Shape, Mesh> _partMeshes = new();
        [SerializeField] private SerializedDictionary<Size, float> _partSizes = new();
        [SerializeField] private bool _overrideParameters = false;
        [SerializeField] private Shape _overrideShape = Shape.Rectangular;
        [SerializeField] private Size _overrideSize = Size.Small;

        public Mesh GetPartMesh(Shape shape, Size size, float length)
        {
            var shapeToUse = _overrideParameters ? _overrideShape : shape;
            var sizeToUse = _overrideParameters ? _overrideSize : size;
            return ExtendMesh(ScaleToSize(CreateMesh(_partMeshes[shapeToUse]), sizeToUse), length);
        }

        private Mesh CreateMesh(Mesh baseMesh)
        {
            var mesh = new Mesh
            {
                vertices = baseMesh.vertices,
                triangles = baseMesh.triangles,
                uv = baseMesh.uv,
                colors = baseMesh.colors,
                normals = baseMesh.normals,
                tangents = baseMesh.tangents
            };
            
            return mesh;
        }

        private Mesh ScaleToSize(Mesh baseMesh, Size size)
        {
            var scale = _partSizes[size];
            Vector3 ModiferFunction(Vector3 vertex) => new (vertex.x, vertex.y * scale, vertex.z * scale);
            return ModifyMesh(ModiferFunction, baseMesh);
        }

        private static Mesh ExtendMesh(Mesh baseMesh, float length)
        {
            Vector3 ModiferFunction(Vector3 vertex) => new(vertex.x * length, vertex.y, vertex.z);
            return ModifyMesh(ModiferFunction, baseMesh);
        }

        private static Mesh ModifyMesh(Func<Vector3, Vector3> modifier, Mesh baseMesh)
        {
            var vertices = baseMesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = modifier(vertices[i]);
            }
            baseMesh.vertices = vertices;
            baseMesh.RecalculateBounds();
            return baseMesh;
        }
    }
}