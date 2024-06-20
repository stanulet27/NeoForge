using System.Collections;
using NeoForge.Deformation.JSON;
using NeoForge.Deformation.Scoring;
using NeoForge.UI.Inventory;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class PartDetails
    {
        public readonly MaterialItem.StartingOption StartingMesh;
        public readonly MaterialData Material;
        public readonly CraftableParts DesiredMesh;
        public readonly ItemWithBonus Coal;

        public float Hits;
        public float InitialScore = -1;
        public MeshSimilarityCalculator.ScoringDetails ScoreDetails;
        
        public PartDetails()
        {
            StartingMesh = MaterialItem.StartingOption.BasicBar;
            Material = MaterialData.CreateDefault();
            DesiredMesh = CraftableParts.BasicBar;
            Coal = null;
            Hits = 0;
        }
        
        public PartDetails(MaterialItem.StartingOption startingMesh, MaterialData material, 
            CraftableParts desiredMesh, ItemWithBonus coal)
        {
            StartingMesh = startingMesh;
            Material = material;
            DesiredMesh = desiredMesh;
            Coal = coal;
            Hits = 0;
        }
        
        public void SetMaterialData(JSONMaterial jsonMaterial)
        {
            Material.MaximumForce = jsonMaterial.MaximumDeformation;
        }

        public IEnumerator SetScoreDetails(ForgedPart part)
        {
            ScoreDetails = new MeshSimilarityCalculator.ScoringDetails();
            yield return ScoreDetails.Setup(part);
        }
    }

    public enum CraftableParts
    {
        None = -1,
        BasicBar = 0,
        Sphere = 1,
    }
}