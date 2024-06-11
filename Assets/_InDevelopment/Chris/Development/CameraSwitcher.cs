using UnityEngine;
using Cinemachine;

namespace _InDevelopment.Chris.Development
{
    public class CameraSwitcher : MonoBehaviour
    {
       [SerializeField] private Animator animator;

        void Start()
        {
           
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                animator.Play("Overview Position");
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                animator.Play("Heating Position");
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                animator.Play("Forging Position");
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                animator.Play("Cooling Position");
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                animator.Play("Planning Position");
            }
        }
    }
}
