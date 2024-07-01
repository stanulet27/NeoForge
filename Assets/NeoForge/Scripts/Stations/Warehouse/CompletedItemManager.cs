using System.Collections.Generic;
using System.Linq;
using NeoForge.UI.Inventory;

namespace NeoForge.Stations.Warehouse
{
    public class CompletedItemManager
    {
        private List<CompletedItem> _completedItems = new();
        private int _currentIndex;
        private bool OutOfRange => _currentIndex < 0 || _currentIndex >= _completedItems.Count;

        /// <summary>
        /// Will return a list of all completed items in the inventory.
        /// </summary>
        public static List<CompletedItem> GetCompletedItems()
        {
            return InventorySystem.Instance.GetItems<CompletedItem>().Where(x => x.Value > 0).Select(x => x.Key)
                .ToList();
        }

        /// <summary>
        /// Will return the current item being viewed. If _currentIndex is out of range, will return null.
        /// </summary>
        public CompletedItem GetCurrentItem()
        {
            return OutOfRange ? null : _completedItems[_currentIndex];
        }

        /// <summary>
        /// Will update the list of completed items to point to the provided list.
        /// </summary>
        public void SetCompletedItems(List<CompletedItem> items)
        {
            _completedItems = items;
        }

        /// <summary>
        /// Will set the current index to the provided index.
        /// </summary>
        public void SetCurrentIndex(int index)
        {
            _currentIndex = index;
        }

        /// <summary>
        /// Will return the next index in the list of completed items. Will wrap around if the end of the list is
        /// reached.
        /// </summary>
        public int GetNextIndex()
        {
            return (_currentIndex + 1 + _completedItems.Count) % _completedItems.Count;
        }
    }
}