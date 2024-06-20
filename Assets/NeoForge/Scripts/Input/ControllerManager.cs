using System;
using System.Collections;
using System.Linq;
using NeoForge.UI.Scenes;
using NeoForge.Utilities;
using SharedData;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NeoForge.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class ControllerManager : SingletonMonoBehaviour<ControllerManager>
    {
        private const int DELAY_BETWEEN_MODE_SWAP = 2;
        
        public static Action<Mode> OnModeSwapped;
        public enum Mode {Gameplay, UI}

        public Mode CurrentMode
        {
            get
            {
                Initialize();
                return Enum.Parse<Mode>(_playerInput.currentActionMap.name);
            }
        }

        private PlayerInput _playerInput;
        private bool _isInitialized;
        private bool _isSlowDown;
        private int _frameOfLastSwap;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        private void Start()
        {
            OnSceneTransitionStart(SceneTools.CurrentSceneIndex);
            SceneTools.OnSceneTransitionStart += OnSceneTransitionStart;
        }

        private void OnDestroy()
        {
            SceneTools.OnSceneTransitionStart -= OnSceneTransitionStart;
        }

        private void Update()
        {
            HandleSlowDownInput();
        }
        
        public string GetKey(string keyName, InputBinding.DisplayStringOptions displayOption = 0)
        {
            try
            {
                var controller = Gamepad.current != null ? "Gamepad" : "Keyboard";
                return _playerInput.actions.FindAction(keyName).bindings
                    .Where(x => x.path.Contains(controller))
                    .Select(x => x.ToDisplayString(displayOption))
                    .First();
            }
            catch { Debug.LogError("Can't find key " + keyName); return "?"; }
        }
        
        public string GetLongKey(string keyName)
        {
            return GetKey(keyName, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
        }

        public IEnumerator AllowUserToSetKey(string keyName)
        {
            _playerInput.enabled = false;
            var operation = _playerInput.actions.FindAction(keyName).PerformInteractiveRebinding();
            
            Debug.Log("Press a key");
            operation.Start();
            yield return new WaitUntil(() => operation.completed);
            operation.Dispose();
            _playerInput.enabled = true;
        }

        private void HandleSlowDownInput()
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
            Initialize();
            _playerInput.SwitchCurrentActionMap(newMode.ToString());
            OnModeSwapped?.Invoke(newMode);
            Debug.Log($"Swapped to {newMode}");
            _frameOfLastSwap = Time.frameCount;
        }
        
        private void TryInvoke(Action action)
        {
            if (Time.frameCount - _frameOfLastSwap < DELAY_BETWEEN_MODE_SWAP) return;
            action?.Invoke();
        }

        #region Gameplay
        public static Action<Vector2> OnMove;
        public static Action<Vector3> OnRotate;
        public static Action OnSwapCamera;
        public static Action OnSwapMode;
        public static Action<bool> OnSlowDown;
        public static Action OnHit;
        public static Action OnOverlay;
        public static Action<Station> OnChangeStation;
        public static Action OnInteract;
        public static Action OnNextDay;
        public static Action OnMouseClick;
        public static Action OnSwapArea;

        public void OnOverlayInput(InputValue context)
        {
            TryInvoke(OnOverlay);
        }
        
        public void OnMoveInput(InputValue context)
        {
            TryInvoke(() => OnMove?.Invoke(context.Get<Vector2>()));
        }
        
        public void OnRotateInput(InputValue context)
        {
            TryInvoke(() => OnRotate?.Invoke(context.Get<Vector3>()));
        }
        
        public void OnChangeToOverviewInput(InputValue context)
        {
            TryInvoke(() => OnChangeStation?.Invoke(Station.Overview));
        }
        public void OnChangeToHeatingInput(InputValue context)
        {
            TryInvoke(() => OnChangeStation?.Invoke(Station.Heating));
        }
        
        
        public void OnChangeToCoolingInput(InputValue context)
        {
            TryInvoke(() => OnChangeStation?.Invoke(Station.Cooling));
        }
        
        public void OnChangeToForgingInput(InputValue context)
        {
            TryInvoke(() => OnChangeStation?.Invoke(Station.Forging));
        }
        
        public void OnChangeToPlanningInput(InputValue context)
        {
            TryInvoke(() => OnChangeStation?.Invoke(Station.Planning));
        }

        public void OnSwapAreaInput()
        {
            TryInvoke(OnSwapArea);
        }
        
        public void OnSwapCameraInput()
        {
            TryInvoke(OnSwapCamera);
        }
        
        public void OnSwapMovementInput()
        {
            TryInvoke(OnSwapMode);
        }
        
        public void OnHitInput()
        {
            TryInvoke(OnHit);
        }
        
        public void OnInteractInput()
        {
            TryInvoke(OnInteract);
        }
        
        public void OnNextDayInput()
        {
            TryInvoke(OnNextDay);
        }

        public void OnMouseClickInput()
        {
            TryInvoke(OnMouseClick);
        }
        #endregion

        #region UI
        public static Action OnGoBack;
        public static Action OnConfirm;
        public static Action OnCancel;
        public static Action OnClose;
        public static Action OnPause;
        public static Action OnSkipDialogue;
        public static Action OnNextDialogue;
        
        public void OnGoBackInput()
        {
            TryInvoke(OnGoBack);
        }
        
        public void OnConfirmInput()
        {
            TryInvoke(OnConfirm);
        }
        public void OnCancelInput()
        {
            TryInvoke(OnCancel);
        }

        public void OnCloseInput()
        {
            TryInvoke(OnClose);
        }

        public void OnPauseInput()
        {
            TryInvoke(OnPause);
        }

        public void OnSkipDialogueInput()
        {
            TryInvoke(OnSkipDialogue);
        }

        public void OnNextDialogueInput()
        {
            OnNextDialogue?.Invoke();
            TryInvoke(OnNextDialogue);
        }
        #endregion
    }
}