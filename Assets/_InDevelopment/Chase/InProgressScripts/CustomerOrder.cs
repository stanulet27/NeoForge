using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NeoForge.Dialogue;
using NeoForge.Dialogue.Helper;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NeoForge.Orders
{
    [Serializable]
    public class CustomerOrder
    {
        [ValueDropdown("_names")] public string CustomerName;
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