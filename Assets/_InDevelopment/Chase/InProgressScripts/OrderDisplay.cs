using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NeoForge.Orders
{
    public class OrderDisplay : MonoBehaviour
    {
        public Action<OrderDisplay> OnOrderClicked;

        [SerializeField] private MeshRenderer _paperRenderer;
        [SerializeField] private Material _paperMaterial;
        [SerializeField] private Material _highlightMaterial;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        
        private Vector3 _initialPosition;
        
        private void Awake()
        {
            _initialPosition = transform.position;
        }
        
        public void SetHighlight(bool highlight)
        {
            _paperRenderer.material = highlight ? _highlightMaterial : _paperMaterial;
        }

        private void OnMouseDown()
        {
            OnOrderClicked?.Invoke(this);
        }

        public void JumpToPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        public void ReturnToInitialPosition()
        {
            transform.position = _initialPosition;
        }
        
        public void SetOrder(Order order)
        {
            gameObject.SetActive(order != null);
            if (order == null) return;
            _title.text = $"<u>{order.Title}</u>";
            _description.text = GenerateDescription(order);
        }

        private string GenerateDescription(Order order)
        {
            var description = "";
            description += $"Customer: {order.GiverName}\n";
            description += $"Payment: {order.PaymentAmount}gp\n";
            description += "Due Date: TBA\n";
            description += "\n";
            description += $"Requirements:\n{order.Requirements}";
            return description;
        }
    }
}