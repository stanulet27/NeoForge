using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Utilities.Movement;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class ForgedPart : MonoBehaviour
    {
        private const float ROOM_TEMPERATURE_KELVIN = 290f;
        private const float TEMPERATURE_CHANGE_RATE = 1f;
        
        public enum PartState
        {
            Heating,
            Cooling,
            Ambient
        }
        
        [Tooltip("The material on the part containing the blackbody radiation shader")]
        [SerializeField] private MeshRenderer _material;

        /// <summary>
        /// The temperature of the part in kelvin
        /// </summary>
        public float Temperature => _temperature;
        
        /// <summary>
        /// The current state of the part
        /// </summary>
        public PartState CurrentState => _currentState;

        /// <summary>
        /// The position of the pary when it is in the furnace
        /// </summary>
        public Transform InFurnacePosition => _inFurnacePosition;
        
        ///<summary>
        /// The position of the part when it is out of the furnace
        /// </summary>
        public Transform OutFurnacePosition => _outFurnacePosition;
        
        private List<PartBoundsLocker> _boundsLocker;
        private PartState _currentState;
        private float _temperature;

        //Heating positions must be generated for each part
        //since this is where multiple parts can be stores
        private Transform _outFurnacePosition;
        private Transform _inFurnacePosition;


        private void Start()
        {
            _currentState = PartState.Ambient;
            _temperature = ROOM_TEMPERATURE_KELVIN;
            _boundsLocker = GetComponentsInChildren<PartBoundsLocker>().ToList();
            _material.material.SetFloat("_Temperature", _temperature);

            _outFurnacePosition = new GameObject("@OutFurnacePosition").transform;
            _inFurnacePosition = new GameObject("@InFurnacePosition").transform;
            //generate positions for in and out of furnace
            if (FurnaceSpotLocator.ReserveNewPosition(this, out Vector3 position))
            {
                _outFurnacePosition.position = position;
                _inFurnacePosition.position = position;
            }

            ChangePosition(_outFurnacePosition);
        }

        /// <summary>
        /// Changes the position of the part to be inside the furnace and starts heating the part
        /// </summary>
        public void StartHeating()
        {
            _currentState = PartState.Heating;
            ChangePosition(_inFurnacePosition);
            StartCoroutine(Heat());
        }

        /// <summary>
        /// Returns the Bounding box of the part
        /// </summary>
        /// <returns>The bounding box of the part</returns>
        public Bounds GetBounds()
        {
            return _material.bounds;
        }

        /// <summary>
        /// Changes the position of the part to be inside the water and starts cooling the part
        /// </summary>
        /// <param name="position">The transform that is inside the water</param>
        public void StartCooling(Transform position)
        {
            _currentState = PartState.Cooling;
            ChangePosition(position);
            StartCoroutine(Cool());
        }

        /// <summary>
        /// Removes the part from either the furnace or the water and stops heating and cooling
        /// </summary>
        /// <param name="coolPosition"></param>
        public void SetToAmbient(Transform coolPosition)
        {
            ChangePosition(_currentState == PartState.Heating ? _inFurnacePosition : coolPosition);
            StopAllCoroutines();
            _currentState = PartState.Ambient;
        }

        /// <summary>
        /// Changes the position of the part to the target position
        /// </summary>
        /// <param name="target">A transform describing the target position and rotation</param>
        public void ChangePosition(Transform target)
        {
            _boundsLocker.ForEach(x => x.enabled = false);
            transform.position = target.position;
            transform.rotation = target.rotation;
            _boundsLocker.ForEach(x => x.enabled = true);
        }

        private IEnumerator Heat()
        {
            float startTime = Time.realtimeSinceStartup;
            while (_currentState == PartState.Heating)
            {
                _temperature += TEMPERATURE_CHANGE_RATE;
                _material.material.SetFloat("_Temperature", _temperature);
                yield return new WaitForSeconds(Time.fixedDeltaTime);
            }
        }

        private IEnumerator Cool()
        {
            while (_currentState == PartState.Cooling)
            {
                _temperature -= TEMPERATURE_CHANGE_RATE;
                _material.material.SetFloat("_Temperature", _temperature);
                yield return new WaitForSeconds(Time.fixedDeltaTime);
                
            }
        }
    }
}