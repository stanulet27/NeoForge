using System;
using NeoForge.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeoForge.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class ControllerManager : SingletonMonoBehaviour<ControllerManager>
    {
        public static Action<Vector2> OnMove;
        public static Action<Vector3> OnRotate;
        public static Action OnSwapCamera;
        public static Action OnSwapMode;
        public static Action<bool> OnSlowDown;
        public static Action OnHit;
        public static Action OnOverlay;
        public static Action<int> OnChangeStation;
        
        public void OnOverlayInput(InputAction.CallbackContext context)
        {
            OnOverlay?.Invoke();
        }
        
        public void OnMoveInput(InputAction.CallbackContext context)
        {
            OnMove?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void OnRotateInput(InputAction.CallbackContext context)
        {
            OnRotate?.Invoke(context.ReadValue<Vector3>());
        }
        
        
        public void OnSwapCameraInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSwapCamera?.Invoke();
            }
        }
        
        public void OnSwapModeInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSwapMode?.Invoke();
            }
        }
        
        public void OnSlowDownInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnSlowDown?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnSlowDown?.Invoke(false);
            }
        }
        
        public void OnHitInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnHit?.Invoke();
            }
        }
        
        public void OnChangeStationInput(InputAction.CallbackContext context)
        {

            int pressed = int.Parse(context.control.name);
            OnChangeStation?.Invoke(pressed);
        }
    }
}