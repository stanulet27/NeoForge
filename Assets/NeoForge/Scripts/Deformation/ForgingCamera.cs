using SharedData;
using UnityEngine;

namespace NeoForge.Deformation
{
    public class ForgingCamera : MonoBehaviour
    {
        [Tooltip("The animator that controls the camera movement between stations")]
        [SerializeField] private Animator _animator;
        
        /// <summary>
        /// Will swap to the camera view of the specified station
        /// </summary>
        public void ChangeCameraView(ForgeArea forgeArea)
        {
            _animator.Play($"{forgeArea.ToString()} Position");
        }
    }
}