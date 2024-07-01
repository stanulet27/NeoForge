using NeoForge.UI.Inventory;
using UnityEngine;

namespace NeoForge.Deformation.JSON
{
    [System.Serializable]
    public class EnvironmentChoices
    {
        public int StartMeshID;
        public int EndMeshID;
        public int MaterialID;
        public int HammerID;

        public EnvironmentChoices(int startMeshID, int endMeshID, int materialID, int hammerID = 0)
        {
            StartMeshID = startMeshID;
            EndMeshID = endMeshID;
            MaterialID = materialID;
            HammerID = hammerID;
        }

        public EnvironmentChoices(PartDetails details)
        {
            Debug.Log("Settings: " + details.StartingMesh + " " + details.DesiredMesh + " " + details.Material.MaterialType);
            StartMeshID = (int) details.StartingMesh;
            EndMeshID = (int) details.DesiredMesh;
            MaterialID = (int) details.Material.MaterialType;
            HammerID = 0;
        }
    }
}