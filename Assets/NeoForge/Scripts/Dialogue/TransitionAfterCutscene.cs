using NeoForge.UI.Scenes;
using UnityEngine;

namespace NeoForge.Dialogue
{
    public class TransitionAfterCutscene : MonoBehaviour
    {
        private void Start()
        {
            DialogueManager.OnDialogueEnded += HandleDialogueEnd;
        }

        private void OnDestroy()
        {
            DialogueManager.OnDialogueEnded -= HandleDialogueEnd;
        }
        
        private void HandleDialogueEnd()
        {
            StartCoroutine(SceneTools.TransitionToScene(SceneTools.NextSceneExists ? SceneTools.NextSceneIndex : 0));
        }
    }
}