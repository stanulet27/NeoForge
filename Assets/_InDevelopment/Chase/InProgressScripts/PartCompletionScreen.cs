using System;
using System.Collections;
using System.Collections.Generic;
using NeoForge.Deformation;
using NeoForge.Deformation.Scoring;
using NeoForge.Input;
using NeoForge.UI.Warehouse;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public class PartCompletionScreen : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The game object for the part completion screen")]
        [SerializeField] private GameObject _display;
        [Tooltip("The text displaying the part name")]
        [SerializeField] private TMP_Text _partName;
        [Tooltip("The text displaying the part details: hits, machine cost, accuracy, coal bonus")]
        [SerializeField] private TMP_Text _details;
        [Tooltip("The text displaying the score")]
        [SerializeField] private TMP_Text _score;
        [Tooltip("The mesh filter displaying the part image")]
        [SerializeField] private PartViewer _partViewer;
        [Tooltip("The text displaying the click to continue message")]
        [SerializeField] private TMP_Text _clickToContinue;

        [Header("Options")]
        [Tooltip("Time in seconds between displaying each detail")]
        [SerializeField, LabelWidth(180)] private float _delayBetweenDetails = 1f;
        [Tooltip("Time in seconds between displaying the details and the score")]
        [SerializeField, LabelWidth(180)] private float _delayBetweenScore = 1f;
        
        [Header("Audio")]
        [Tooltip("The audio source for the sound effects")]
        [SerializeField] private AudioSource _audioSource;
        [Tooltip("The sound effect for displaying the details, played after each detail")]
        [SerializeField] private AudioClip _detailsSound;
        [Tooltip("The sound effect for displaying the score, played after the details")]
        [SerializeField] private AudioClip _scoreSound;

        private ControllerManager.Mode _lastMode;
        private Action _onClose;

        private void Start()
        {
            _lastMode = ControllerManager.Instance.CurrentMode;
            Hide();
        }

        private void OnDestroy()
        {
            ControllerManager.OnConfirm -= Hide;
        }

        [Button]
        private void Test()
        {
            Display(new ForgingResults { PartName = "Copper Bar", PartMade = null, Hits = 10, 
                MachineCost = 15, Accuracy = 0.97f, CoalBonus = 0.2f});
        }
        
        /// <summary>
        /// Displays the given results to the user, will invoke the provided callback when the user confirms
        /// </summary>
        public void Display(ForgingResults results, Action onClose = null)
        {
            _lastMode = ControllerManager.Instance.CurrentMode;
            ControllerManager.Instance.SwapMode(ControllerManager.Mode.UI);
            _onClose = onClose;
            _partName.text = results.PartName;
            _display.SetActive(true);
            _partViewer.DisplayPart(results.PartMade);
            StartCoroutine(DisplayDetails(results));
        }
        
        /// <summary>
        /// Will hide the completion screen from the user and invoke the close callback
        /// </summary>
        public void Hide()
        {
            _display.SetActive(false);
            _details.text = "";
            _score.text = "";
            _clickToContinue.gameObject.SetActive(false);
            _onClose?.Invoke();
            
            ControllerManager.Instance.SwapMode(_lastMode);
            ControllerManager.OnConfirm -= Hide;
        }

        private IEnumerator DisplayDetails(ForgingResults results)
        {
            var details = new List<string>
            {
                $"{results.Hits}\n",
                $"{results.MachineCost:N2}\n",
                $"{results.Accuracy:N2}\n",
                $"{results.CoalBonus:N2}",
            };
            
            foreach (var detail in details)
            {
                yield return new WaitForSeconds(_delayBetweenDetails);
                _audioSource.PlayOneShot(_detailsSound);
                _details.text += detail;
            }

            yield return new WaitForSeconds(_delayBetweenScore);
            _audioSource.PlayOneShot(_scoreSound);
            _score.text = results.Score;
            
            _clickToContinue.gameObject.SetActive(true);
            ControllerManager.OnConfirm += Hide;
        }

        public class ForgingResults
        {
            [Tooltip("The name of the part that was made")]
            public string PartName;
            [Tooltip("The total hits used to create the part")]
            public float Hits;
            [Tooltip("The bonus from the coal used")]
            public float CoalBonus;
            [Tooltip("The cost of the machine used to create the part")]
            public float MachineCost;
            [Tooltip("The accuracy of the part")]
            public float Accuracy;
            [Tooltip("The mesh of the part that was made")]
            public Mesh PartMade;
            
            public string Score => CalculateScore();

            public ForgingResults() {}

            public ForgingResults(MeshSimilarityCalculator similarityCalculator, PartDetails details, Mesh partMade)
            {
                PartName = details.DesiredMesh.ToString().SplitPascalCase();
                Hits = details.Hits;
                MachineCost = similarityCalculator.OvershotScore;
                Accuracy = similarityCalculator.Score;
                PartMade = partMade;
                CoalBonus = DetermineCoalBonus(details.Coal);
            }

            private float DetermineCoalBonus(ItemWithBonus item)
            {
                if (item == null) return 0;
                return item.Modifies switch
                {
                    ItemWithBonus.BonusTarget.FinalScore => item.ApplyBonus(Accuracy),
                    ItemWithBonus.BonusTarget.MachineCost => item.ApplyBonus(MachineCost),
                    _ => 0
                };
            }
            
            private string CalculateScore()
            {
                var score = Accuracy + CoalBonus - Hits / 2 - (100 - MachineCost) / 20f;
                return score switch
                {
                    >= 100 => "S",
                    >= 90 => "A",
                    >= 80 => "B",
                    >= 70 => "C",
                    >= 60 => "D",
                    _ => "F"
                };
            }
        }
    }
}