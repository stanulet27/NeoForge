using System;
using System.Collections;
using System.Collections.Generic;
using NeoForge.Dialogue;
using NeoForge.Input;
using SharedData;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace NeoForge.Orders
{
    public class OrderGenerator : MonoBehaviour, IStation
    {
        [SerializeField] private List<DailyOrders> _dailyOrders;
        //[SerializeField] private GameObject _customerSpawnPoint;
        [SerializeField] private SharedInt _currentDay;
        [SerializeField] private SharedBool _readyForNextDay;
        [SerializeField] private GameObject _customerShell;
        [SerializeField] private UnityEvent _onLastCustomerServed;
        
        private DailyOrders _todaysOrders;
        private string _dialogueToTrigger;
        private readonly List<GameObject> _skins = new();
        
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
            StartCoroutine(GenerateNextOrder());
        }
        
        private void OnDestroy()
        {
            ControllerManager.OnInteract -= InteractWithCustomer;
            DialogueManager.OnDialogueEnded -= OnCustomerServed;
        }

        private void OnCustomerServed()
        {
            DialogueManager.OnDialogueEnded -= OnCustomerServed;
            Debug.Log("Customer served");
            _customerShell.SetActive(false);
            StartCoroutine(GenerateNextOrder());
        }
        
        private void InteractWithCustomer()
        {
            if (string.IsNullOrWhiteSpace(_dialogueToTrigger)) return;
            Debug.Log("interacting with customer");
            
            DialogueManager.Instance.StartDialogueName(_dialogueToTrigger);
            DialogueManager.OnDialogueEnded += OnCustomerServed;
            _dialogueToTrigger = "";
        }
        
        private IEnumerator GenerateNextOrder()
        {
            if (_todaysOrders.TryGetOrder(out var order))
            {
                yield return new WaitForSeconds(Random.Range(3, 8));
                _skins.ForEach(x => x.SetActive(x.name.StartsWith(order.CustomerName, StringComparison.InvariantCultureIgnoreCase)));
                _customerShell.SetActive(true);
                _dialogueToTrigger = order.Dialogue;
            }
            else
            {
                _readyForNextDay.Value = true;
                _onLastCustomerServed?.Invoke();
            }
        }

        public void EnterStation()
        {
            ControllerManager.OnInteract += InteractWithCustomer;
        }

        public void ExitStation()
        {
            ControllerManager.OnInteract -= InteractWithCustomer;
        }
    }
}