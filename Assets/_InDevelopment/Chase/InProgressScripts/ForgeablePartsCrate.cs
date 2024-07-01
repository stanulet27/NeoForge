using NeoForge.UI.Inventory;
using TMPro;
using UnityEngine;

namespace NeoForge.Stations.Warehouse
{
    public class ForgeablePartsCrate : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private string _emptyLabel;
        [SerializeField] private Renderer _meshRenderer;
        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private Material _highlightMaterial;

        private void OnEnable()
        {
            _label.text = _emptyLabel;
            _meshRenderer.material = _defaultMaterial;
        }

        public void SetPart(MaterialItem part)
        {
            var noPart = part == null;
            _label.text = noPart ? _emptyLabel : part.Name;
            _meshRenderer.material = noPart ? _defaultMaterial : _highlightMaterial;
        }
    }
}