using NeoForge.UI.Buttons;
using UnityEngine;

namespace NeoForge.UI.Sound
{
    [RequireComponent(typeof(UIButton))]
    public class ButtonSound : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<UIButton>().OnClick += PlaySound;
        }

        private static void PlaySound(IButton _)
        {
            UISounds.Instance.Play();
        }
        
        private void OnDestroy()
        {
            GetComponent<UIButton>().OnClick -= PlaySound;
        }
    }
}