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
        
        private PlayerInput _playerInput;
        private bool _isInitialized;
        private bool _isSlowDown;
        
        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Start()
        {
            OnSceneTransitionStart(SceneTools.CurrentSceneIndex);
            SceneTools.onSceneTransitionStart += OnSceneTransitionStart;
        }

        private void OnDestroy()
        {
            SceneTools.onSceneTransitionStart -= OnSceneTransitionStart;
        }

        private void Update()
        {
            var newValue = _playerInput.actions.FindAction("SlowDownInput").IsPressed();
            if (newValue == _isSlowDown) return;
            
            OnSlowDown?.Invoke(newValue);
            _isSlowDown = newValue;
        }

        private void OnSceneTransitionStart(int index)
        {
            Initialize();
            SwapMode(index == 0 ? Mode.UI : Mode.Gameplay);
        }
        
        // ReSharper disable Unity.PerformanceAnalysis - Will only be called once
        private void Initialize()
        {
            if (_isInitialized) return;
            _playerInput = GetComponent<PlayerInput>();
            _isInitialized = true;
        }

        /// <summary>
        /// Will swap the current input map of the player input to the given mode.
        /// </summary>
        /// <param name="newMode">The mode to swap to</param>
        public void SwapMode(Mode newMode)
        {
            _playerInput.SwitchCurrentActionMap(newMode.ToString());
        }

        #region Gameplay
        public static Action<Vector2> OnMove;
        public static Action<Vector3> OnRotate;
        public static Action OnSwapCamera;
        public static Action OnSwapMode;
        public static Action<bool> OnSlowDown;
        public static Action OnHit;

        public void OnMoveInput(InputValue context)
        {
            OnMove?.Invoke(context.Get<Vector2>());
        }
        
        public void OnRotateInput(InputValue context)
        {
            OnRotate?.Invoke(context.Get<Vector3>());
        }
        
        public void OnSwapCameraInput()
        {
            OnSwapCamera?.Invoke();
        }
        
        public void OnSwapModeInput()
        {
            OnSwapMode?.Invoke();
        }
        
        public void OnHitInput()
        {
            OnHit?.Invoke();
        }
        #endregion

        #region UI
        public static Action OnGoBack;
        public static Action OnConfirm;
        public static Action OnCancel;
        public static Action OnClose;
        public static Action OnPause;
        
        public void OnGoBackInput()
        {
            OnGoBack?.Invoke();
        }
        
        public void OnConfirmInput()
        {
            OnConfirm?.Invoke();
        }
        
        public void OnCancelInput()
        {
            OnCancel?.Invoke();
        }
        
        public void OnCloseInput()
        {
            OnClose?.Invoke();
        }
        
        public void OnPauseInput()
        {
            OnPause?.Invoke();
        }
        #endregion
    }
}