using NeoForge.Deformation;
using NeoForge.Deformation.Scoring;
using Sirenix.Utilities;
using UnityEngine;

namespace NeoForge.UI.Inventory
{
    public class ForgingResults
    {
        /// <summary>
        /// The name of the part that was made
        /// </summary>
        public string PartName;
        
        /// <summary>
        /// The total hits used to create the part
        /// </summary>
        public float Hits;
        
        /// <summary>
        /// The bonus from the coal used
        /// </summary>
        public float CoalBonus;
        
        /// <summary>
        /// The cost from overshot for making the part,
        /// represents cost for machining down the part to the correct size
        /// </summary>
        public float MachineCost;
        
        /// <summary>
        /// The accuracy of the part
        /// </summary>
        public float Accuracy;
        
        /// <summary>
        /// The mesh of the part that was made
        /// </summary>
        public Mesh PartMade;
        
        /// <summary>
        /// The part that was being attempted to be made
        /// </summary>
        public readonly CraftableParts PartGoal;

        /// <summary>
        /// The score of the part made
        /// </summary>
        public PartScore Score => CalculateScore();

        public ForgingResults() {}
        
        public ForgingResults(MeshSimilarityCalculator similarityCalculator, PartDetails details, Mesh partMade)
        {
            PartName = details.DesiredMesh.ToString().SplitPascalCase();
            Hits = details.Hits;
            MachineCost = MeshSimilarityCalculator.MachineCostScore;
            Accuracy = similarityCalculator.Score;
            PartMade = partMade;
            CoalBonus = DetermineCoalBonus(details.Coal);
            PartGoal = details.DesiredMesh;
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
            
        private PartScore CalculateScore()
        {
            var score = Accuracy + CoalBonus - Hits / 2 - (100 - MachineCost) / 20f;
            return score switch
            {
                >= 100 => PartScore.S,
                >= 90 => PartScore.A,
                >= 80 => PartScore.B,
                >= 70 => PartScore.C,
                >= 60 => PartScore.D, 
                _ => PartScore.F
            };
        }
    }
}