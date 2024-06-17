using System;
using TMPro;
using UnityEngine;

namespace NeoForge.Orders
{
    public class OrderDisplay : MonoBehaviour
    {
        /// <summary>
        /// Triggered when the order is clicked by the mouse.
        /// </summary>
        public Action<OrderDisplay> OnOrderClicked;
        
        [Header("Material Properties")]
        [Tooltip("The renderer of the paper")]
        [SerializeField] private MeshRenderer _paperRenderer;
        [Tooltip("The material of the paper when not selected")]
        [SerializeField] private Material _paperMaterial;
        [Tooltip("The material of the paper when selected")]
        [SerializeField] private Material _highlightMaterial;
        
        [Header("Text Properties")]
        [Tooltip("The title of the order")]
        [SerializeField] private TMP_Text _title;
        [Tooltip("The details of the order")]
        [SerializeField] private TMP_Text _description;
        
        private Vector3 _initialPosition;
        
        private void Awake()
        {
            _initialPosition = transform.position;
        }
        
        /// <summary>
        /// Will modify the material of the paper to show if it is highlighted or not.
        /// </summary>
        public void SetHighlight(bool highlight)
        {
            _paperRenderer.material = highlight ? _highlightMaterial : _paperMaterial;
        }

        /// <summary>
        /// Will move the order to the given position.
        /// </summary>
        public void JumpToPosition(Vector3 position)
        {
            transform.position = position;
        }

        /// <summary>
        /// Will move the order back to it originally was when the scene started.
        /// </summary>
        public void ReturnToInitialPosition()
        {
            transform.position = _initialPosition;
        }
        
        /// <summary>
        /// Will set the order to the given order. If the order is null, the display will be hidden.
        /// </summary>
        public void SetOrder(Order order)
        {
            var isAnOrder = order != null;
            
            gameObject.SetActive(isAnOrder);
            _title.text = isAnOrder ? $"<u>{order.Title}</u>" : "";
            _description.text = isAnOrder ? GenerateDescription(order) : "";
        }

        private void OnMouseDown()
        {
            OnOrderClicked?.Invoke(this);
        }
        
        private static string GenerateDescription(Order order)
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