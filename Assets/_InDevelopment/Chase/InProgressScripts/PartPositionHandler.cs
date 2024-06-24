using System.Collections.Generic;
using System.Linq;
using NeoForge.Utilities.Movement;
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

        /// <summary>
        /// The position of the part when it is out of the furnace in the heating area
        /// </summary>
        public Transform OutFurnacePosition { get; private set; }
        
        /// <summary>
        /// The position of the part when it is in the furnace in the heating area
        /// </summary>
        public Transform InFurnacePosition { get; private set; }

        private void Awake()
        {
            OutFurnacePosition = new GameObject("@OutFurnacePosition").transform;
            InFurnacePosition = new GameObject("@InFurnacePosition").transform;
            OutFurnacePosition.SetParent(transform.parent);
            InFurnacePosition.SetParent(transform.parent);
            GetComponentsInChildren(_boundsLocker);
            GetComponentsInChildren(_moveables);
        }
        
        private void OnDisable()
        {
            FurnaceSpotLocator.VacatePosition(this);
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
            
            OutFurnacePosition.SetPositionAndRotation(position, rotation);
            InFurnacePosition.SetPositionAndRotation(position + shiftTowardsFurnace, rotation);
            JumpToPosition(OutFurnacePosition);
        }
        
        /// <summary>
        /// Changes the position of the part to the target position
        /// </summary>
        /// <param name="target">A transform describing the target position and rotation</param>
        public void JumpToPosition(Transform target)
        {
            _boundsLocker.ForEach(x => x.enabled = false);
            transform.position = target.position;
            transform.rotation = target.rotation;
            _boundsLocker.ForEach(x => x.enabled = true);
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
        /// Returns the Bounding box of the part
        /// </summary>
        /// <returns>The bounding box of the part</returns>
        public Bounds GetBounds()
        {
            return _bounds.bounds;
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
        
        private static void AdjustPosition(Transform moveable, float offset)
        {
            var position = moveable.localPosition;
            position.y = offset;
            moveable.localPosition = position;
        }
    }
}