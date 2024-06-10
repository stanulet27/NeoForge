using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace NeoForge.UI.Tools
{
    public class ButtonVisibilityFeatureToggler : MonoBehaviour
    {
        [Tooltip("The game objects to toggle the visibility of. The key is the game object and the value is the visibility theme.")]
        [SerializeField] private SerializedDictionary<GameObject, VisibilityTheme> _visibilities;
        
        private AnimatorEventListener _broadcaster;
        
        public void Awake()
        {
            SetVisibility("Normal");
            _broadcaster = GetComponent<AnimatorEventListener>();
        }
        
        private void Start()
        {
            _broadcaster.OnStateEnter += SetVisibility;
        }

        private void OnDestroy()
        {
            _broadcaster.OnStateEnter -= SetVisibility;
        }

        private void SetVisibility(string state)
        {
            if (!VisibilityTheme.IsValidState(state)) return;
            
            foreach (var obj in _visibilities.Keys)
            {
                obj.SetActive(_visibilities[obj].GetValueFromState(state));
            }
        }
    }
}