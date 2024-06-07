using UnityEngine;

namespace NeoForge.Dialogue
{
    public class VisibleBasedOnWorldState : MonoBehaviour
    {
        [Tooltip("The key of the world state property to check")]
        [SerializeField] private string _key;
        [Tooltip("The value the world state property should be")]
        [SerializeField] private int _value;
        [Tooltip("Will cause the object to be visible if the world state property is not equal to the value")]
        [SerializeField] private bool _invert;
        
        private void Start()
        {
            WorldState.OnWorldStateChanged += UpdateVisibility;
            UpdateVisibility();
        }

        private void OnDestroy()
        {
            WorldState.OnWorldStateChanged -= UpdateVisibility;
        }

        private void UpdateVisibility()
        {
            var criteriaMet = WorldState.GetState(_key) == _value;
            gameObject.SetActive(criteriaMet != _invert);
        }
    }
}