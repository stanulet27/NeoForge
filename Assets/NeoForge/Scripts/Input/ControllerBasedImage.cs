using UnityEngine;
using UnityEngine.UI;

namespace NeoForge.Input
{
    [RequireComponent(typeof(Image))]
    public class ControllerBasedImage : ImageBasedOnInputSourceBase<Image>
    {
        protected override void SetDisplay(Sprite sprite)
        {
            Display.sprite = sprite;
        }
    }
}