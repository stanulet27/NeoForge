using TMPro;
using UnityEngine;

namespace NeoForge.Orders
{
    public class OrderUIDisplay : MonoBehaviour
    {
        private const string NO_ORDER_TEXT = "No orders yet, check back later!";
        [SerializeField] private GameObject _display;
        [SerializeField] private TMP_Text _orderDescriptionField;

        /// <summary>
        /// Will display the description of the order supplied. If no order is supplied, will display a default message
        /// </summary>
        /// <param name="description"></param>
        public void Display(string description = NO_ORDER_TEXT)
        {
            description ??= NO_ORDER_TEXT;
            _orderDescriptionField.text = description;
            _display.SetActive(true);
        }

        /// <summary>
        /// Will hide the display of the current order
        /// </summary>
        public void Hide()
        {
            _display.SetActive(false);
        }
    }
}