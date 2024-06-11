using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace _InDevelopment.Chris.Development
{
    public class StationCameraController : MonoBehaviour
    {
        [SerializeField] private Transform _overviewPosition;
        [SerializeField] private Transform _planningPosition;
        [SerializeField] private Transform _forgingPosition;
        [SerializeField] private Transform _heatingPosition;
        [SerializeField] private Transform _coolingPosition;

        private void Start()
        {
            transform.position = _overviewPosition.position;
            transform.rotation = _overviewPosition.rotation;
        }

        [ContextMenu("Move to overview")]
        public void MoveToOverview()
        {
            StartCoroutine(MoveToPosition(_overviewPosition));
        }
        
        [ContextMenu("Move to planning")]
        public void MoveToPlanning()
        {
            StartCoroutine(MoveToPosition(_planningPosition));
        }
        
        [ContextMenu("Move to forging")]
        public void MoveToForging()
        {
            StartCoroutine(MoveToPosition(_forgingPosition));
        }
        
        [ContextMenu("Move to heating")]
        public void MoveToHeating()
        {
            StartCoroutine(MoveToPosition(_heatingPosition));
        }
        
        [ContextMenu("Move to cooling")]
        public void MoveToCooling()
        {
            StartCoroutine(MoveToPosition(_coolingPosition));
        }

        private IEnumerator MoveToPosition(Transform target, float time = 1f)
        {
            yield return new WaitForSeconds(0.01f);
            var startPosition = transform.position;
            var startRotation = transform.rotation;
            var startTime = Time.time;
            var endTime = startTime + time;
            while (Time.time < endTime)
            {
                var t = (Time.time - startTime) / time;
                transform.position = Vector3.Lerp(startPosition, target.position, t);
                transform.rotation = Quaternion.Lerp(startRotation, target.rotation, t);
                yield return null;
            }
            transform.position = target.position;
            transform.rotation = target.rotation;
        }

    }
}