using NeoForge.Deformation.JSON;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class JAXEnvironmentSettings
    {
        public readonly Transform Part;
        public readonly PartMeshHandler MeshData;
        public readonly PartDetails Details;
        public readonly EnvironmentChoices Environment;
        
        public JAXEnvironmentSettings(Transform part, PartMeshHandler meshData, PartDetails details)
        {
            Part = part;
            MeshData = meshData;
            Details = details;
            Environment = new EnvironmentChoices(details);
        }
    }
}