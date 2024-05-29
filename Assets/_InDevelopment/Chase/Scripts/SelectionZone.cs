using System.Linq;
using UnityEngine;

namespace DeformationSystem
{
    /*
     * This class is attached to the camera so that it can fire a ray cast straight ahead.
     * If the raycast intersects with anything in layer "Player" then it will move the game object provided
     * to the intersection point
     */
    
    public class SelectionZone : MonoBehaviour
    {
        [Tooltip("The object that will be moved to the intersection point.")]
        [SerializeField] private GameObject _selectedObject;
        [SerializeField] private GameObject _noHitObject;
        [SerializeField] private float _size;

        private Vector3[] raysFired =
        {
            Vector3.zero,
            Vector3.forward,
            Vector3.back,
            Vector3.left,
            Vector3.right,
            Vector3.forward + Vector3.left,
            Vector3.forward + Vector3.right,
            Vector3.back + Vector3.left,
            Vector3.back + Vector3.right
        };
        
        private void Update()
        {
            _selectedObject.SetActive(false);
            _noHitObject.SetActive(true);
            
            foreach (var ray in raysFired.Select(x => x * _size / 2f))
            {
                if (!CheckWithOffset(ray, out var hitPoint)) continue;
                _selectedObject.transform.position = hitPoint;
                _selectedObject.SetActive(true);
                _noHitObject.SetActive(false);
                break;
            }
            
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