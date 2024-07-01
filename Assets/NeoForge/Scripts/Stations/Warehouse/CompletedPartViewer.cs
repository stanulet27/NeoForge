using NeoForge.Dialogue;
using NeoForge.Input;
using NeoForge.Stations.Orders;
using NeoForge.UI.Inventory;
using UnityEngine;

namespace NeoForge.Stations.Warehouse
{
    public class CompletedPartViewer : MonoBehaviour
    {
        private enum SelectionResult
        {
            Failure,
            Success,
            SuccessBonus
        }
        
        [Tooltip("The UI to use to display the completed parts.")]
        [SerializeField] private CompletedPartViewerUI _viewerUI;
        
        private ViewerStateManager _stateManager;
        private CompletedItemManager _itemManager;
        private Order _order;
        private string _dialogueOnClose = "";
        
        private bool DialogueIsPrimed => !string.IsNullOrEmpty(_dialogueOnClose);

        private void Start()
        {
            _itemManager = new CompletedItemManager();
            _stateManager = new ViewerStateManager();
            _stateManager.OnStateChanged += HandleStateChanged;
            _stateManager.ChangeState(ViewerState.Hidden);
        }
        
        /// <summary>
        /// Will open the menu to view the completed parts.
        /// </summary>
        public void Open()
        {
            _stateManager.ChangeState(ViewerState.Viewing);
        }

        /// <summary>
        /// Will open the menu to view the completed parts and allow the player to select a part to complete an order.
        /// </summary>
        /// <param name="dialogueEvent">
        /// The order that the part is being used to fulfill,
        /// format: {prefix}-{giverName}-{desiredPart}
        /// </param>
        public void OpenForSelection(string dialogueEvent)
        {
            _stateManager.ChangeState(ViewerState.ViewingSelection);
            _order = OrderController.Instance.GetOrder(dialogueEvent.Split("-")[1], dialogueEvent.Split("-")[2]);
            _dialogueOnClose = $"{_order.GiverName}-Cancel";
        }

        /// <summary>
        /// Will close the menu.
        /// </summary>
        public void Hide()
        {
            _stateManager.ChangeState(ViewerState.Hidden);
        }

        /// <summary>
        /// Will move to the next item in the list and display it.
        /// </summary>
        public void Next()
        {
            Debug.Log("Going to next item");
            
            if (_stateManager.CurrentState is not ViewerState.Hidden)
            {
                DisplayItem(_itemManager.GetNextIndex());
            }
        }

        /// <summary>
        /// Will select the current item being displayed, determine if it is the correct part to complete the order,
        /// update the inventory and order accordingly, give the user payment, and close the menu.
        /// </summary>
        public void Select()
        {
            if (_stateManager.CurrentState is not ViewerState.ViewingSelection) return;
            if (_order == null) return;

            var item = _itemManager.GetCurrentItem();
            if (item == null) return;

            Debug.Log($"Selecting {item.Name} for order {_order.GiverName}-{_order.ObjectToCraft}");
            
            var result = ValidateOrder(item);
            ProcessSelectionResult(result, item);

            Hide();
        }

        private void HandleStateChanged(ViewerState newState)
        {
            Debug.Log($"Entering {newState}");
            
            switch (newState)
            {
                case ViewerState.Hidden:
                    OnEnterHiddenState();
                    break;
                case ViewerState.Viewing:
                    OnEnterViewingState();
                    break;
                case ViewerState.ViewingSelection:
                    OnEnterViewingSelectionState();
                    break;
            }
        }

        private void OnEnterViewingState()
        {
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            var completedItems = CompletedItemManager.GetCompletedItems();
            _itemManager.SetCompletedItems(completedItems);

            _viewerUI.UpdateButtons(nextButtonActive: completedItems.Count > 1, selectButtonActive: false);
            _order = null;
            _dialogueOnClose = "";

            DisplayItem(0);
            _viewerUI.SetDisplayActive(true);
        }

        private void OnEnterViewingSelectionState()
        {
            OnEnterViewingState();
            var itemCount = CompletedItemManager.GetCompletedItems().Count;
            _viewerUI.UpdateButtons(nextButtonActive: itemCount > 1, selectButtonActive: itemCount > 0);
        }
        
        private void OnEnterHiddenState()
        {
            _viewerUI.SetDisplayActive(false);
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.Gameplay);

            if (DialogueIsPrimed)
            {
                DialogueManager.Instance.StartDialogueName(_dialogueOnClose);
            }
        }

        private SelectionResult ValidateOrder(CompletedItem item) =>
            item.Goal != _order.ObjectToCraft ? SelectionResult.Failure :
            item.Score <= _order.BonusScore ? SelectionResult.SuccessBonus :
            item.Score <= _order.MinimumScore ? SelectionResult.Success :
            SelectionResult.Failure;

        private void ProcessSelectionResult(SelectionResult result, ItemBase item)
        {
            string suffix;
            int payment;

            switch (result)
            {
                case SelectionResult.SuccessBonus:
                    suffix = "SuccessBonus";
                    payment = _order.PaymentWithBonus;
                    break;
                case SelectionResult.Success:
                    suffix = "Success";
                    payment = _order.PaymentAmount;
                    break;
                case SelectionResult.Failure:
                default:
                    suffix = "Failure";
                    payment = 0;
                    break;
            }

            _dialogueOnClose = $"{_order.GiverName}-{suffix}";
            InventorySystem.Instance.CurrentGold += payment;

            if (result is SelectionResult.Success or SelectionResult.SuccessBonus)
            {
                InventorySystem.Instance.RemoveItem(item);
                OrderController.Instance.CompleteOrder(_order);
            }

            Debug.Log("Selected " + item.Name);
        }

        private void DisplayItem(int index)
        {
            _itemManager.SetCurrentIndex(index);
            var item = _itemManager.GetCurrentItem();
            if (item != null)
            {
                _viewerUI.UpdatePartName(item.Name);
                _viewerUI.DisplayPart(item.Mesh);
            }
            else
            {
                _viewerUI.UpdatePartName("No Parts Made");
                _viewerUI.DisplayPart(null);
            }
        }
    }
}