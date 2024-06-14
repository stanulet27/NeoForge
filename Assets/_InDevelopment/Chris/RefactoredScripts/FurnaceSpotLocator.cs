using System.Collections.Generic;
using NeoForge.Deformation;
using UnityEngine;

public static class FurnaceSpotLocator
{
    private const int MAX_SPOTS = 4;
    private const float MAX_DISTANCE = 15f;
    private const float BUFFER= 0.5f;
    
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
    /// <returns>True if position was reserved, false otherwise</returns>
    public static bool ReserveNewPosition(ForgedPart part, out Vector3 position)
    {
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
        var lastPart = _storedParts[_storedParts.Count - 1];
        var lastBoundingBox = lastPart.GetBounds();
        var lastPosition = lastPart.transform.position;
        var lastSize = lastBoundingBox.size;
        
        var currentPart = part.GetBounds();
        var currentSize = currentPart.size;
        
        var newPosition = lastPosition + new Vector3(lastSize.x/2 + currentSize.x/2 + BUFFER, 0, 0);
        if(newPosition.x - _initialLocation.position.x >= MAX_DISTANCE)
        {
            position = new Vector3();
            return false;
        }
        
        _storedParts.Add(part);
        position = newPosition;
        return true;
    }

    //TODO: reorganize the parts when a part is removed (completed)
    public static void VacatePosition()
    {
        if (_storedParts.Count == 0) return;
    }
    
    
}
