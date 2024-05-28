﻿using UnityEngine;

namespace DeformationSystem
{
    /*
     * This class is attached to the camera so that it can fire a ray cast straight ahead.
     * If the raycast intersects with anything in layer "Player" then it will move the game object provided
     * to the intersection point
     */
    
    public class SelectionZone : MonoBehaviour
    {
        [SerializeField] private GameObject selectedObject;
        
        private void Update()
        {
            var hitOccured = 
                Physics.Raycast(transform.position, transform.forward, out var hit, Mathf.Infinity, LayerMask.GetMask("Part"));

            selectedObject.transform.position = hitOccured ? hit.point : transform.position + transform.forward * 10f;
            selectedObject.SetActive(hitOccured);
        }
    }
}