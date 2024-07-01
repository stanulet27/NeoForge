using System;
using AYellowpaper.SerializedCollections;
using NeoForge.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.Deformation.JSON
{
    public class PartMeshDatabase : SingletonMonoBehaviour<PartMeshDatabase>
    {
        private const float STEEL_DENSITY = .284f; // density of steel in lb/ft^3
        private const float INCHES_TO_METERS = 0.0254f;

        public enum Shape { Rectangular, Spherical }
        public enum Size { Small, Medium, Large }
        
        [Tooltip("The mesh frames for different shapes that will serve as the base.")]
        [SerializeField] private SerializedDictionary<Shape, Mesh> _partMeshes = new();
        [Tooltip("The different face sizes that the parts can be.")]
        [SerializeField] private SerializedDictionary<Size, float> _partSizes = new();
        
        [Header("Debug Properties")]
        [Tooltip("When true, the parameters will be overridden and will use the below parameters.")]
        [SerializeField] private bool _overrideParameters;
        [ShowIf("_overrideParameters")]
        [Tooltip("The shape to use when the parameters are overridden.")]
        [SerializeField] private Shape _overrideShape = Shape.Rectangular;
        [ShowIf("_overrideParameters")]
        [Tooltip("The size to use when the parameters are overridden.")]
        [SerializeField] private Size _overrideSize = Size.Small;

        /// <summary>
        /// Will return a newly created mesh that represents the part with the given details.
        /// </summary>
        public Mesh GetPartMesh(PartMeshDetails details)
        {
            var shapeToUse = _overrideParameters ? _overrideShape : details.Shape;
            var sizeToUse = _overrideParameters ? _overrideSize : details.Size;
            var length = DetermineLengthFromWeight(details);
            return ExtendMesh(ScaleToSize(CreateMesh(_partMeshes[shapeToUse]), sizeToUse), length);
        }

        private static Mesh CreateMesh(Mesh baseMesh)
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
            var scale = _partSizes[size] * INCHES_TO_METERS;
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
        
        private float DetermineLengthFromWeight(PartMeshDetails details)
        {
            float CylindricalBilletAreaFunction(float diameter) => Mathf.PI * Mathf.Pow(diameter / 2, 2);
            float RectangularBilletAreaFunction(float sideLength) => Mathf.Pow(sideLength, 2);
            
            var weight = details.Weight;
            var length = _partSizes[details.Size];
            
            return details.Shape switch
            {
                Shape.Rectangular => CalculateBilletLength(RectangularBilletAreaFunction, weight, length),
                Shape.Spherical => CalculateBilletLength(CylindricalBilletAreaFunction, weight, length),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static float CalculateBilletLength(Func<float, float> areaFunction, float weight, float diameter)
        {
            var volumeInCubicInches = weight / STEEL_DENSITY;
            var lengthInInches = volumeInCubicInches / areaFunction(diameter);

            var lengthInMeters = lengthInInches * INCHES_TO_METERS;

            return lengthInMeters;
        }
    }
}