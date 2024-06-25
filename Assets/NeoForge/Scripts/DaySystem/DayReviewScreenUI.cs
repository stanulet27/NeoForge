using System.Collections.Generic;
using NeoForge.Input;
using NeoForge.Stations.Orders;
using NeoForge.UI.Inventory;
using NeoForge.UI.Scenes;
using SharedData;
using TMPro;
using UnityEngine;

namespace NeoForge.DaySystem
{
    public class DayReviewScreenUI : MonoBehaviour
    {
        [Tooltip("The parent object that contains the day review screen.")]
        [SerializeField] private GameObject _display;
        [Tooltip("The text field that will display the current day.")]
        [SerializeField] private TMP_Text _dayTextField;
        [Tooltip("The text field that will display the renown earned today.")]
        [SerializeField] private TMP_Text _renownTextField;
        [Tooltip("The text field that will display the gold earned today.")]
        [SerializeField] private TMP_Text _goldTextField;
        [Tooltip("The group that contains the order review displays.")]
        [SerializeField] private GameObject _orderReviewGroup;
        [Tooltip("The group that contains the part review displays.")]
        [SerializeField] private GameObject _partReviewGroup;

        [Tooltip("The shared int that tracks the current day.")]
        [SerializeField] private SharedInt _dateTracker;

        private readonly List<OrderReviewDisplay> _orderReviewDisplays = new();
        private readonly List<PartReviewDisplay> _partReviewDisplays = new();

        private void Awake()
        {
            _orderReviewGroup.GetComponentsInChildren(_orderReviewDisplays);
            _partReviewGroup.GetComponentsInChildren(_partReviewDisplays);
        }

        private void Start()
        {
            Hide();
        }

        private void OnDestroy()
        {
            ControllerManager.OnConfirm -= ProceedToNextDay;
        }

        /// <summary>
        /// Will display the day review screen and allow the player to proceed to the next day.
        ///
        /// Will display the current day, the renown and gold earned today, the orders due today,
        /// and the parts made today.
        ///
        /// Will swap the controller mode to UI.
        /// </summary>
        public void Display()
        {
            Debug.Log("Displaying Day Review Screen");
            
            _dayTextField.text = $"Day {_dateTracker.ValueAsText()}";
            DisplayInventoryStatus();
            DisplayOrders();
            
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            ControllerManager.OnConfirm += ProceedToNextDay;
            _display.SetActive(true);
        }
        
        private void Hide()
        {
            _display.SetActive(false);
        }

        private void DisplayInventoryStatus()
        {
            var renownEarned = DayManager.Instance.RenownEarnedToday;
            var goldEarned = DayManager.Instance.GoldEarnedToday;
            var currentRenown = InventorySystem.Instance.CurrentRenown;
            var currentGold = InventorySystem.Instance.CurrentGold;

            _renownTextField.text = $"{currentRenown - renownEarned} => {currentRenown}";
            _goldTextField.text = $"{currentGold - goldEarned} => {currentGold}";
            
            DisplayParts();
        }

        private void ProceedToNextDay()
        {
            Debug.Log("Proceeding to next day");
            
            ControllerManager.OnConfirm -= ProceedToNextDay;
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);
            StartCoroutine(SceneTools.TransitionToScene(1));
        }

        private void DisplayOrders()
        {
            var orders = OrderController.Instance.GetActiveOrders();
            
            for (var i = 0; i < _orderReviewDisplays.Count; i++)
            {
                if (i < orders.Count)
                {
                    _orderReviewDisplays[i].Display(orders[i], orders[i].DueDate - (int)_dateTracker);
                }
                else
                {
                    _orderReviewDisplays[i].Hide();
                }
            }
        }
        
        private void DisplayParts()
        {
            var itemsMade = DayManager.Instance.CompletedPartsMadeToday;

            for (var i = 0; i < _partReviewDisplays.Count; i++)
            {
                if (i < itemsMade.Count)
                {
                    _partReviewDisplays[i].Display(itemsMade[i]);
                }
                else
                {
                    _partReviewDisplays[i].Hide();
                }
            }
        }
    }
}