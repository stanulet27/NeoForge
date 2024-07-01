using System.Linq;
using NeoForge.UI.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NeoForge.UI.Journal
{
    public class JournalPage : MonoBehaviour
    {
        [Tooltip("Name of the material")]
        [SerializeField] private TMP_Text _title;
        [Tooltip("The column that will display the temperatures")]
        [SerializeField] private TMP_Text _temperatureColumn;
        [Tooltip("The column that will display the minimum times till the part is at the desired temperature")]
        [SerializeField] private TMP_Text _minTimeColumn;
        [Tooltip("The column that will display the maximum times till the part over heats")]
        [SerializeField] private TMP_Text _maxTimeColumn;
        [Tooltip("The column of values representing the properties of the material; " +
                 "heated strength, normal strength, and cost")]
        [SerializeField] private TMP_Text _properties;
        [Tooltip("The flavor text of the material")]
        [SerializeField] private TMP_Text _flavorText;
        [Tooltip("The sketch of the material")]
        [SerializeField] private Image _sketch;

        /// <summary>
        /// Will display the material information on the journal page and make the page visible.
        /// </summary>
        public void DisplayMaterial(MaterialData data)
        {
            gameObject.SetActive(true);
            _title.text = data.Name;
            _sketch.sprite = data.Icon;
            DisplayTemperatureData(data.TemperatureInfos);
            _properties.text = $"{data.HeatedStrength}\n{data.NormalStrength}\n{data.Cost}gp";
            _flavorText.text = data.FlavorText;
        }
        
        /// <summary>
        /// Will hide the journal page.
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        private void DisplayTemperatureData(MaterialData.TemperatureInfo[] info)
        {
            _temperatureColumn.text = info.Select(x => x.Temperature).Aggregate("Temp", (x, y) => $"{x}\n{y}c");
            _minTimeColumn.text = info.Select(x => x.MinimumTime).Aggregate("Min Time", (x, y) => $"{x}\n{y} sec");
            _maxTimeColumn.text = info.Select(x => x.MaximumTime).Aggregate("Max Time", (x, y) => $"{x}\n{y} sec");
        }
    }
}