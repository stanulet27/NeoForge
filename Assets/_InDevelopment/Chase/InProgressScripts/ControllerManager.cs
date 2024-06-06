using System;
using NeoForge.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeoForge.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class ControllerManager : SingletonMonoBehaviour<ControllerManager>
    {
        public enum Mode {Gameplay, UI}

        private void Start()
        {
            SceneTools.onSceneTransitionStart += index => SwapMode(index == 0 ? Mode.UI : Mode.Gameplay);
        }

        #region Gameplay
        public static Action<Vector2> OnMove;
        public static Action<Vector3> OnRotate;
        public static Action OnSwapCamera;
        public static Action OnSwapMode;
        public static Action<bool> OnSlowDown;
        public static Action OnHit;
        
        

        public void SwapMode(Mode newMode)
        {
            GetComponent<PlayerInput>().SwitchCurrentActionMap(newMode.ToString());
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
        #endregion

        #region UI
        public static Action OnGoBack;
        public static Action OnConfirm;
        public static Action OnCancel;
        public static Action OnClose;
        public static Action OnPause;
        
        public void OnGoBackInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnGoBack?.Invoke();
            }
        }
        
        public void OnConfirmInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnConfirm?.Invoke();
            }
        }
        
        public void OnCancelInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnCancel?.Invoke();
            }
        }
        
        public void OnCloseInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnClose?.Invoke();
            }
        }
        
        public void OnPauseInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OnPause?.Invoke();
            }
        }
        #endregion
    }
}