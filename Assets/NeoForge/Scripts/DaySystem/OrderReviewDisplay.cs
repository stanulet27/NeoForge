using NeoForge.Stations.Orders;
using TMPro;
using UnityEngine;

namespace NeoForge.DaySystem
{
    public class OrderReviewDisplay : MonoBehaviour
    {
        [Tooltip("The text field that will display the order details.")]
        [SerializeField] private TMP_Text _orderTextField;

        /// <summary>
        /// Will display the order details and the days left until the order is due.
        /// Format: "> {orderTitle} due in {daysLeft} days" or "> {orderTitle} due tomorrow" or "> {orderTitle} FAILED"
        /// </summary>
        public void Display(Order order, int daysLeft)
        {
            var dayText = 
                daysLeft > 1 ? $"due in {daysLeft} days" :
                daysLeft > 0 ? "due tomorrow" : 
                "FAILED";
            
            _orderTextField.text = $"> {order.Title} {dayText}";
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Will cause the display to no longer be visible and clear the displayed order.
        /// </summary>
        public void Hide()
        {
            _orderTextField.text = "";
            gameObject.SetActive(false);
        }
    }
}