using System;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Dialogue;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.Orders
{
    [Serializable]
    public class CustomerOrder
    {
        [Tooltip("The name of the customer that is ordering the item. Used to determine the skin.")]
        [ValueDropdown("_names")] public string CustomerName;
        
        [Tooltip("The dialogue that is triggered when speaking with the customer.")]
        [ValueDropdown("GetDialogues")] public string Dialogue;

        private readonly string[] _names =
        {
            "Bartender", "Fairy", "Headsman", "Hermit", "Jester",
            "King", "Mage", "Merchant", "Monk", "Nun", "PeasantF", "PeasantM",
            "Priest", "Prince", "Princess", "Queen", "Rider", "SoliderF", "SoliderM",
        };

        private IEnumerable<string> GetDialogues()
        {
            var dialogues = Resources.LoadAll<ConversationDataSO>("Dialogue");
            return dialogues.Select(x => x.Data.ID).Distinct();
        }
    }
}