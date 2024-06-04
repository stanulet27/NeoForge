using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NeoForge.Deformation
{
    [RequireComponent(typeof(Collider))]
    public class TriggerTracker : MonoBehaviour
    {
        private Collider _collider;
        private readonly List<GameObject> _containedObjects = new();
        
        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!_containedObjects.Contains(other.gameObject)) _containedObjects.Add(other.gameObject);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (_containedObjects.Contains(other.gameObject)) _containedObjects.Remove(other.gameObject);
        }
        
        public float GetSize()
        {
            return _collider.bounds.size.magnitude * transform.lossyScale.magnitude;
        }

        /// <summary>
        /// Determines whether the trigger tracker contains a point.
        /// </summary>
        /// <param name="point">The location of the point</param>
        /// <returns>True if contained in the trigger tracker's collider, otherwise false</returns>
        public bool Contains(Vector3 point)
        {
            return _collider.bounds.Contains(point);
        }
        
        /// <summary>
        /// Gets all the objects currently touching the trigger tracker of a specific type.
        /// </summary>
        /// <typeparam name="T">Which types of objects being touched to return</typeparam>
        /// <returns>The objects of type T that are currently being touched</returns>
        public IEnumerable<T> GetContainedObjects<T>() where T : Component
        {
            return _containedObjects
                .ConvertAll(obj => obj.GetComponent<T>())
                .Where(component => component != null);
        }
    }
}