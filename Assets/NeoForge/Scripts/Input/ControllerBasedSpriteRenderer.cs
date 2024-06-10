using UnityEngine;

namespace NeoForge.Input
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ControllerBasedSpriteRenderer : ImageBasedOnInputSourceBase<SpriteRenderer>
    {
        protected override void SetDisplay(Sprite sprite)
        {
            Display.sprite = sprite;
        }
    }
}