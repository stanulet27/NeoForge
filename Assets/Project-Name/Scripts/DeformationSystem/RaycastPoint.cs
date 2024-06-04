using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    public class RaycastPoint
    {
        public enum Mode
        {
            Overshot,
            Undershot
        }
        
        public static Dictionary<Mode, List<RaycastPoint>> Points { get; } = new ()
        {
            {Mode.Undershot, new List<RaycastPoint>()},
            {Mode.Overshot, new List<RaycastPoint>()}
        };
        
        public Vector3 Origin => _parent.TransformPoint(_origin);

        private readonly int _layerMask;
        private readonly Transform _parent;
        private readonly Vector3 _origin;

        public RaycastPoint(Vector3 position, Transform parent, Mode mode)
        {
            _parent = parent;
            _origin = position;
            _layerMask = LayerMask.GetMask(mode == Mode.Undershot ? "Desired" : "Part");
            if (IsInCollider()) Points[mode].Add(this);
            _layerMask = LayerMask.GetMask(mode == Mode.Undershot ? "Part" : "Desired");
        }

        public static void ClearPoints(Mode mode)
        {
            Points[mode].Clear();
        }
        
        public static float GetScore(Mode mode)
        {
            return (float)Points[mode].Count(point => point.DoesItScore()) / Points[mode].Count * 100f;
        }
        
        public bool DoesItScore()
        {
            return IsInCollider();
        }

        private bool IsInCollider()
        {
            return RaycastCheck(Vector3.up) && RaycastCheck(Vector3.right);
        }

        private bool RaycastCheck(Vector3 direction)
        {
            var hitCount = 0;

            // Count hits with normals pointing away from the point
            hitCount += CountEdgesBetween(direction * 5000f, Origin); 
            
            // Count hits with normals pointing towards the point
            hitCount += CountEdgesBetween(Origin, Origin + direction * 5000f);

            return hitCount % 2 == 1; // If the number of hits is odd, the point is inside the collider
        }

        private int CountEdgesBetween(Vector3 from, Vector3 to)
        {
            var counter = 0;
            var direction = (to - from).normalized;

            while (true)
            {
                var distance = Vector3.Distance(from, to);
                var noEdgeHit = !Physics.Raycast(from, direction, out var hit, distance, _layerMask);
                if (noEdgeHit) break;
                
                counter++;
                from = hit.point + direction * 0.001f; // Move slightly beyond the hit point
            }

            return counter;
        }
    }
}