using System;
using System.Collections;
using System.Collections.Generic;
using NeoForge.Dialogue;
using NeoForge.Input;
using NeoForge.Stations.Orders;
using SharedData;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace NeoForge.Stations.Customers
{
    public class CustomerManager : MonoBehaviour, IStation
    {
        [Tooltip("The customers that will arrive each day. The order of the list is the order of the days.")]
        [SerializeField] private List<DailyOrders> _dailyOrders;
        [Tooltip("The current day that the player is on. This is used to get the orders for the day.")]
        [SerializeField] private SharedInt _currentDay;
        [Tooltip("If all customers have been served and the day is ready to end early.")]
        [SerializeField] private SharedBool _readyForNextDay;
        [Tooltip("The game object used to display the customer. The skins are the children of this object.")]
        [SerializeField] private GameObject _customerShell;
        [Tooltip("The event to trigger when the last customer has been served.")]
        [SerializeField] private UnityEvent _onLastCustomerServed;
        [Tooltip("Will trigger when a new customer appears.")]
        [SerializeField] private UnityEvent _onNewCustomer;
        
        private readonly List<GameObject> _skins = new();
        private DailyOrders _todaysOrders;
        private string _dialogueToTrigger;
        private Coroutine _summonNextCustomer;

        private void Start()
        {
            Debug.Assert(_currentDay <= _dailyOrders.Count, $"There are no orders reference for day {_currentDay.Value}");
            
            foreach (Transform child in _customerShell.transform)
            {
                _skins.Add(child.gameObject);
            }

            _todaysOrders = _dailyOrders[_currentDay - 1];
            _todaysOrders.PrepareDay();
            _readyForNextDay.Value = false;
            
            _summonNextCustomer = StartCoroutine(TrySpawnNextCustomer());
        }
        
        private void OnDestroy()
        {
            ControllerManager.OnInteract -= InteractWithCustomer;
            DialogueManager.OnDialogueEnded -= OnCustomerServed;
        }
        
        public void EnterStation()
        {
            ControllerManager.OnInteract += InteractWithCustomer;
        }

        public void ExitStation()
        {
            ControllerManager.OnInteract -= InteractWithCustomer;
        }

        /// <summary>
        /// Will block the next customer from coming and instead bring back the last customer served.
        /// </summary>
        public void BlockLeaving()
        {
            DialogueManager.OnDialogueEnded -= OnCustomerServed;
            DialogueManager.OnDialogueEnded += OnCustomerServed;
            StopCoroutine(_summonNextCustomer);
            _customerShell.SetActive(true);
            Debug.Log("Blocking");
        }

        private void OnCustomerServed()
        {
            DialogueManager.OnDialogueEnded -= OnCustomerServed;
            _customerShell.SetActive(false);
            _summonNextCustomer = StartCoroutine(TrySpawnNextCustomer());
            Debug.Log("Customer served");
        }
        
        private void InteractWithCustomer()
        {
            if (string.IsNullOrWhiteSpace(_dialogueToTrigger)) return;

            DialogueManager.OnDialogueEnded += OnCustomerServed;
            DialogueManager.Instance.StartDialogueName(_dialogueToTrigger);
            Debug.Log("Interacting with customer");
        }
        
        private IEnumerator TrySpawnNextCustomer()
        {
            Debug.Log("Checking for next customer");
            yield return null;
            _dialogueToTrigger = "";
            
            if (_todaysOrders.TryGetOrder(out var order))
            {
                Debug.Log("Customer Found...Waiting for next customer to appear");
                yield return new WaitForSeconds(Random.Range(3, 8));
                SpawnCustomer(order);
            }
            else
            {
                Debug.Log("No more customers...Ending day early is now available");
                _readyForNextDay.Value = true;
                _onLastCustomerServed?.Invoke();
            }
        }

        private void SpawnCustomer(CustomerOrder order)
        {
            _skins.ForEach(x => x.SetActive(IsMatchingSkin(x, order.CustomerName)));
            _onNewCustomer?.Invoke();
            _customerShell.SetActive(true);
            _dialogueToTrigger = order.Dialogue;
        }

        private static bool IsMatchingSkin(GameObject skin, string customerName)
        {
            return skin.name.StartsWith(customerName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}