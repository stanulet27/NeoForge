using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomInspectors;
using NeoForge.Deformation;
using NeoForge.UI.Scenes;
using NeoForge.Utilities;
using UnityEngine;

namespace NeoForge.Stations.Warehosue
{
    public class ForgePartPool : SingletonMonoBehaviour<ForgePartPool>
    {
        [Tooltip("Generic part prefab.")]
        [SerializeField] private ForgedPart _forgedPartPrefab;
        [Tooltip("The positions for the forged parts.")]
        [SerializeField] private ForgingPositions _forgingPositions;
        [Tooltip("Total number of parts that can be forged.")]
        [SerializeField] private int _totalParts = 5;

        public List<ForgedPart> ForgedPartsPool { get; } = new();

        private void Start()
        {
            _forgingPositions.Initialize();
            for (var i = 0; i < _totalParts; i++)
            {
                var part = Instantiate(_forgedPartPrefab, transform);
                part.gameObject.SetActive(false);
                part.SetPositions(_forgingPositions);
                part.transform.parent = transform;
                ForgedPartsPool.Add(part);
            }

            SceneTools.OnSceneTransitionStart += OnSceneTransitionStart;
        }
        
        private void OnSceneTransitionStart(int sceneIndex)
        {
            if (sceneIndex != 0) return;
            ForgedPartsPool.ForEach(part => part.gameObject.SetActive(false));
        }

        public ForgedPart GetPart()
        {
            return ForgedPartsPool.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
        }
    }
}