using System;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public class ShopItemWeightScreen : MonoBehaviour
    {
        private const int MIN_BOUNDS = 5;
        private const int MAX_BOUNDS = 50;
        private const int PRICE_PER_POUND = 25;
        
        [SerializeField] GameObject _display;
        [SerializeField] private TMP_InputField _weightTextField;
        [SerializeField] private TMP_Text _paymentButton;
        [SerializeField] private Color _canPayColor;
        [SerializeField] private Color _cannotPayColor;
        
        private string _lastDisplayedWeight;
        private bool _canPay;
        private Action _onClose;
        private TemplateMaterialItem _item;

        public void Start()
        {
            _display.SetActive(false);
        }

        public void OpenWeightScreen(TemplateMaterialItem item, Action onClose)
        {
            _item = item;
            UpdateWeight("25");
            _weightTextField.Select();
            _onClose = onClose;
            _display.SetActive(true);
        }
        
        public void UpdateWeight(string newWeight)
        {
            if (!int.TryParse(newWeight, out var weight))
            {
                UpdateWeight(_lastDisplayedWeight);
                return;
            }
            
            weight = Mathf.Clamp(weight, MIN_BOUNDS, MAX_BOUNDS);
            
            _lastDisplayedWeight = newWeight;
            _weightTextField.text = newWeight + "lbs";
            _canPay = InventorySystem.Instance.CurrentGold >= weight * PRICE_PER_POUND;
            _paymentButton.color = _canPay ? _canPayColor : _cannotPayColor;
        }
        
        public void IncreaseWeight()
        {
            var currentWeight = int.Parse(_lastDisplayedWeight);
            UpdateWeight((currentWeight + 1).ToString());
        }
        
        public void DecreaseWeight()
        {
            var currentWeight = int.Parse(_lastDisplayedWeight);
            UpdateWeight((currentWeight - 1).ToString());
        }
        
        public void TryToPurchase()
        {
            if (!_canPay) return;
            
            var weight = int.Parse(_lastDisplayedWeight);
            var purchasedItem = MaterialItem.CreateMaterial(_item.MaterialData, _item.PartMeshDetails(weight));
            InventorySystem.Instance.AddItem(purchasedItem);
            InventorySystem.Instance.CurrentGold -= weight * PRICE_PER_POUND;
            Close();
        }
        
        public void Close()
        {
            _display.SetActive(false);
            _onClose?.Invoke();
        }
    }
}