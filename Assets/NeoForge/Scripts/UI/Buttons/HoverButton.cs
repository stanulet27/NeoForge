using UnityEngine.EventSystems;

namespace NeoForge.UI.Buttons
{
    public class HoverButton : UIButton, IPointerEnterHandler
    {
        /// <summary>
        /// This is a hover button that does nothing.
        /// </summary>
        public override void Use()
        {
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}