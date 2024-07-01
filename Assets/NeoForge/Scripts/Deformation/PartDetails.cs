using System.Collections;
using NeoForge.Deformation.JSON;
using NeoForge.Deformation.Scoring;
using NeoForge.UI.Inventory;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class PartDetails
    {
        /// <summary>
        /// The part the player is starting from.
        /// </summary>
        public readonly Mesh StartingMesh;
        
        /// <summary>
        /// The material the part is made from.
        /// </summary>
        public readonly MaterialData Material;
        
        /// <summary>
        /// The goal part the player is trying to make.
        /// </summary>
        public readonly CraftableParts DesiredMesh;
        
        /// <summary>
        /// The coal being used to forge the part.
        /// </summary>
        public readonly ItemWithBonus Coal;

        /// <summary>
        /// The number of hits the player has made shaping the part so far.
        /// </summary>
        public float Hits;
        
        /// <summary>
        /// The scoring details for the part used to calculate the score.
        /// </summary>
        public MeshSimilarityCalculator.ScoringDetails ScoreDetails;

        public PartDetails(Mesh startingMesh, MaterialData material, 
            CraftableParts desiredMesh, ItemWithBonus coal)
        {
            StartingMesh = startingMesh;
            Material = material;
            DesiredMesh = desiredMesh;
            Coal = coal;
            Hits = 0;
        }
        
        /// <summary>
        /// Will extract the json data from the material and set the material data to match the new data.
        /// </summary>
        public void SetMaterialData(JSONMaterial jsonMaterial)
        {
            Material.MaximumForce = jsonMaterial.MaximumDeformation;
        }

        /// <summary>
        /// Will setup the scoring details for the part so it can be scored.
        /// </summary>
        public IEnumerator SetScoreDetails(PartMeshHandler part)
        {
            ScoreDetails = new MeshSimilarityCalculator.ScoringDetails();
            yield return ScoreDetails.Setup(part);
        }
    }
}