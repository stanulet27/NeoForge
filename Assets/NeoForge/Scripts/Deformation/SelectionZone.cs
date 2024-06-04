using System.Linq;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class SelectionZone : MonoBehaviour
    {
        [Tooltip("The object that will be moved to the intersection point.")]
        [SerializeField] private GameObject _selectedObject;
        [Tooltip("The object that will be displayed if no part is hit.")]
        [SerializeField] private GameObject _noHitObject;
        [Tooltip("The size of the selection zone. Assumes square zone.")]
        [SerializeField] private float _size;

        // Center and the top 4 corners and side midpoints
        private readonly Vector3[] _raysFired =
        {
            Vector3.zero, Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
            Vector3.forward + Vector3.left, Vector3.forward + Vector3.right,
            Vector3.back + Vector3.left, Vector3.back + Vector3.right
        };
        
        private void Update()
        {
            var rayFoundTarget = false;

            foreach (var ray in _raysFired.Select(x => x * _size / 2f))
            {
                if (!CheckWithOffset(ray, out var hitPoint)) continue;
                _selectedObject.transform.position = hitPoint;
                rayFoundTarget = true;
                break;
            }
            
            _selectedObject.SetActive(rayFoundTarget);
            _noHitObject.SetActive(rayFoundTarget);
        }
        
        private bool CheckWithOffset(Vector3 corner, out Vector3 hitPoint)
        {
            var hitOccured = Physics.Raycast(transform.position + corner, transform.forward, out var hit, Mathf.Infinity, 
                LayerMask.GetMask("Part"));
            hitPoint = hit.point - corner;
            return hitOccured;
        }
    }
}