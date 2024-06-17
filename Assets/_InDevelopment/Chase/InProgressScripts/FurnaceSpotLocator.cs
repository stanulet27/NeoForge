using System.Collections.Generic;
using UnityEngine;

namespace NeoForge.Deformation
{
    public static class FurnaceSpotLocator
    {
        private const int MAX_SPOTS = 4;
        private const float MAX_DISTANCE = 15f;
        private const float BUFFER = 0.50f;

        private static Transform _initialLocation;
        private static List<ForgedPart> _storedParts = new();

        /// <summary>
        /// Sets the initial location of parts outside the furnace
        /// </summary>
        /// <param name="location">The initial location</param>
        public static void SetInitialLocation(Transform location)
        {
            _initialLocation = location;
        }

        /// <summary>
        /// Reserves a position outside the furnace for the part to rest
        /// </summary>
        /// <param name="part">The part that needs the spot (should be this)</param>
        /// <param name="position">Vector that will be zero if spot was unable
        /// to be reserved, or the new position</param>
        /// <param name="rotation">Quaternion that will be the initial rotation</param>
        /// <returns>True if position was reserved, false otherwise</returns>
        public static bool ReserveNewPosition(ForgedPart part, out Vector3 position, out Quaternion rotation)
        {
            rotation = _initialLocation.rotation;
            if (_storedParts.Count == MAX_SPOTS)
            {
                position = new Vector3();
                return false;
            }

            position = _initialLocation.position;
            if (_storedParts.Count == 0)
            {
                _storedParts.Add(part);
                return true;
            }

            var newPosition = DeterminePosition(_storedParts[^1], part);
            if (Vector3.Distance(newPosition, _initialLocation.position) >= MAX_DISTANCE)
            {
                position = new Vector3();
                return false;
            }

            _storedParts.Add(part);
            position = newPosition;
            return true;
        }

        private static Vector3 DeterminePosition(ForgedPart previousPart, ForgedPart newPart)
        {
            var lastPosition = previousPart.transform.position;
            var offsetDirection = previousPart.transform.right;
            var lastSize = previousPart.GetBounds().size;
            var currentSize = newPart.GetBounds().size;

            var offsetAmount = lastSize.x / 2 + currentSize.x / 2 + BUFFER;

            return lastPosition + offsetDirection * offsetAmount;
        }

        //TODO: reorganize the parts when a part is removed (completed)
        public static void VacatePosition(ForgedPart part)
        {
            _storedParts.Remove(part);
            
            for (var i = 0; i < _storedParts.Count; i++)
            {
                var currentPart = _storedParts[i];
                var rotation = _initialLocation.rotation;
                Debug.Log($"Reorganizing parts {currentPart}");
                if (i == 0)
                {
                    currentPart.ResetPosition(_initialLocation.position, rotation);
                    continue;
                }
                
                currentPart.ResetPosition(DeterminePosition(_storedParts[i - 1], currentPart), _initialLocation.rotation);
            }
        }
    }
}