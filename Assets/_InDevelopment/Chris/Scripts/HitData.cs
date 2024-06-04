using System;
using UnityEngine;

namespace DeformationSystem
{
    [Serializable]
    public class HitData
    {        
        public float DeformationAmount;
        public float[] Rotation;
        public float[] Position;

        public int[] TopVertices;
        public int[] BottomVertices;

        public HitData( float deformationAmount, 
                        Quaternion rotation, 
                        Vector3 position, 
                        int[] topVerticies, 
                        int[] bottomVerticies)
        {
            DeformationAmount = deformationAmount;
            Rotation = new[] { rotation.w, rotation.x, rotation.y, rotation.z};
            Position = new[] { position.x, position.y, position.z };
            TopVertices = topVerticies;
            BottomVertices = bottomVerticies;
        }
    }
}