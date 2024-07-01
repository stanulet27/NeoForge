using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Utilities.Movement;
using SharedData;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class PartPositionHandler : MonoBehaviour
    {
        private const float SHIFT_TOWARDS_FURNACE = 2f;
        
        [Tooltip("The bounds of the part")]
        [SerializeField] private MeshRenderer _bounds;
        [Tooltip("The vertical offset of the part to apply when selected")]
        [SerializeField] private float _verticalOffset = 0.5f;

        private readonly List<PartBoundsLocker> _boundsLocker = new();
        private readonly List<Moveable> _moveables = new();
        private ForgingPositions _positions;
        private Transform _outFurnacePosition;
        private Transform _inFurnacePosition;
        
        /// <summary>
        /// Returns the bounding box of the part
        /// </summary>
        public Bounds Bounds() => _bounds.bounds;

        private void Awake()
        {
            _outFurnacePosition = new GameObject("@OutFurnacePosition").transform;
            _inFurnacePosition = new GameObject("@InFurnacePosition").transform;
            _outFurnacePosition.SetParent(transform.parent);
            _inFurnacePosition.SetParent(transform.parent);
            GetComponentsInChildren(_boundsLocker);
            GetComponentsInChildren(_moveables);
        }
        
        private void OnDisable()
        {
            FurnaceSpotLocator.VacatePosition(this);
        }
        
        /// <summary>
        /// Will initialize the forging positions of the part. These are used for knowing where to go when jumping
        /// between stations
        /// </summary>
        public void InitializeForgingPositions(ForgingPositions positions)
        {
            _positions = new ForgingPositions(positions);
        }
        
        /// <summary>
        /// Will try to reserve a position for the part, if it fails it will disable the part
        /// </summary>
        public void SetupPosition()
        {
            if (FurnaceSpotLocator.ReserveNewPosition(this, out var position, out var rotation))
            {
                ResetPosition(position, rotation);
            }
            else
            {
                Debug.Log("Unable to reserve position for part");
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Will reset the position of the part to the target position by adjusting the furnace positions
        /// </summary>
        public void ResetPosition(Vector3 position, Quaternion rotation)
        {
            var shiftTowardsFurnace = Vector3.forward * SHIFT_TOWARDS_FURNACE;
            
            _outFurnacePosition.SetPositionAndRotation(position, rotation);
            _inFurnacePosition.SetPositionAndRotation(position + shiftTowardsFurnace, rotation);
            _positions.PartPositions[ForgeArea.Heating] = _outFurnacePosition;
            JumpToPosition(_outFurnacePosition);
        }
        
        /// <summary>
        /// Will move the part to the station corresponding to the given forge area
        /// </summary>
        public void JumpToStation(ForgeArea forgeArea)
        {
            JumpToPosition(_positions.PartPositions[forgeArea]);
        }
        
        /// <summary>
        /// Will move the part to the in water position.
        /// </summary>
        public void SubmergeInWater()
        {
            JumpToPosition(_positions.InWaterPosition);
        }
        
        /// <summary>
        /// Will move the part to the in furnace position
        /// </summary>
        public void PutIntoFurnace()
        {
            JumpToPosition(_inFurnacePosition);
        }
        
        /// <summary>
        /// Will toggle the movement of the part, when movement is disabled the part will be reset to its original
        /// position.
        /// </summary>
        public void ToggleMovement(bool canMove)
        {
            _moveables.ForEach(x => x.enabled = canMove);
            if (!canMove) _moveables.ForEach(x => x.ResetPosition());
        }

        /// <summary>
        /// Will apply / remove a vertical offset to / from the part
        /// </summary>
        /// <param name="shouldOffset">When true, will apply an offset. When false, will remove the offset</param>
        public void ToggleVerticalOffset(bool shouldOffset)
        {
            _moveables.ForEach(x =>
            {
                if (shouldOffset) AdjustPosition(x.transform, _verticalOffset);
                else x.ResetPosition();
            });
        }
        
        private void JumpToPosition(Transform target)
        {
            _boundsLocker.ForEach(x => x.enabled = false);
            transform.position = target.position;
            transform.rotation = target.rotation;
            _boundsLocker.ForEach(x => x.enabled = true);
        }
        
        private static void AdjustPosition(Transform moveable, float offset)
        {
            var position = moveable.localPosition;
            position.y = offset;
            moveable.localPosition = position;
        }
    }
}