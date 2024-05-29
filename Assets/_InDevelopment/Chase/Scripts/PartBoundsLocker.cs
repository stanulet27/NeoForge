﻿using System;
using DeformationSystem;
using UnityEngine;

namespace _InDevelopment.Chase.Scripts
{
    public class PartBoundsLocker : MonoBehaviour
    {
        [Tooltip("X, Y, Z bounds. Will prevent the part from moving outside of these bounds. Based in world origin")]
        [SerializeField] private FloatRange[] _bounds;
        
        [SerializeField] private bool _fromStartingPosition;

        Vector3 _startingPosition;
        
        private void Awake()
        {
            _startingPosition = transform.position;
        }
        
        private void Update()
        {
            ClampPosition();
        }
        
        private void ClampPosition()
        {
            var position = transform.position;
            for (int i = 0; i < 3; i++)
            {
                position[i] = Mathf.Clamp(position[i], _bounds[i].Min + _startingPosition[i], _bounds[i].Max + _startingPosition[i]);
            }
            transform.position = position;
        }
    }
}