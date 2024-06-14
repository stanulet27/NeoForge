using System.Collections;
using System.Collections.Generic;
using NeoForge.UI.Buttons;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NeoForge.UI.Tools
{
    public class AutoScroller : MonoBehaviour
    {
        [Tooltip("The duration of the scroll animation")]
        [SerializeField] private float _scrollDuration = 0.5f;
        [Tooltip("The transform that is the parent of the content")]
        [SerializeField] private RectTransform _scrollRect;
        [Tooltip("The transform that contains the buttons")]
        [SerializeField] private RectTransform _content;
        
        private GameObject _currentSelection;

        private void OnEnable()
        {
            StartCoroutine(HandleAutoScroll());
        }
        
        private IEnumerator HandleAutoScroll()
        {
            if (EventSystem.current == null)
            {
                Debug.LogWarning("No EventSystem found, auto scroll will not work");
                yield break;
            }

            while (true)
            {
                yield return new WaitUntil(() => !IsSelected(_currentSelection));
                _currentSelection = EventSystem.current.currentSelectedGameObject;
                if (NeedsToBeScrolledTo(_currentSelection, out var selection))
                {
                    StartCoroutine(ScrollToView(selection));
                }
            }
        }

        private bool NeedsToBeScrolledTo(GameObject selection, out RectTransform rect)
        {
            rect = null;
            return selection != null 
                   && selection.TryGetComponent(out rect)
                   && rect.parent == _content
                   && !IsVisible(rect);
        }
        
        private IEnumerator ScrollToView(RectTransform buttonRect)
        { 
            var contentPos = _content.position;
            var selection = _currentSelection;
            var elapsedTime = 0f;
            var offset = Container.DetermineOffset(_scrollRect, buttonRect);
            var targetPos = contentPos + Vector3.up * offset;
            
            while (elapsedTime < _scrollDuration && selection == _currentSelection)
            {
                _content.position = Vector3.Lerp(contentPos, targetPos, elapsedTime / _scrollDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        private static bool IsSelected(Object x)
        {
            return x == EventSystem.current.currentSelectedGameObject;
        }
        
        private bool IsVisible(RectTransform buttonRect)
        {
            var button = new Container(buttonRect);
            var scroll = new Container(_scrollRect);
            
            return button.Top < scroll.Top && button.Bottom > scroll.Bottom;
        }

        private class Container
        {
            private float Position { get; }
            private float Height { get; }
            public float Top { get; }
            public float Bottom { get; }
            
            public Container(RectTransform rectTransform)
            {
                Position = rectTransform.position.y;
                Height = rectTransform.sizeDelta.y;
                Top = Position + Height / 2;
                Bottom = Position - Height / 2;
            }

            public static float DetermineOffset(RectTransform start, RectTransform end)
            {
                return DetermineOffset(new Container(start), new Container(end));
            }

            public static float DetermineOffset(Container start, Container end)
            {
                return end.Position > start.Position
                    ? start.Top - end.Top
                    : start.Bottom - end.Bottom;
            }
        } 
    }
}