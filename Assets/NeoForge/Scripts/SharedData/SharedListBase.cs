/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SharedData
{
    /// <summary>
    ///     This serves as scriptable object that can be used to have a shared list of type T between objects
    /// </summary>
    public abstract class SharedListBase<T> : ScriptableObject, IEnumerable<T>
    {
        protected abstract List<T> _elements { get; set; }

        public T this[int i]
        {
            get => _elements[i];
            set
            {
                _elements[i] = value;
                OnValueChanged?.Invoke();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event Action OnValueChanged;

        public void AddElement(T element)
        {
            if (_elements == null) Debug.Log("no elements");
            if (element == null) Debug.Log("no element");
            _elements.Add(element);
            OnValueChanged?.Invoke();
        }

        public void Insert(int position, T element)
        {
            _elements.Insert(position, element);
            OnValueChanged?.Invoke();
        }

        public void RemoveElement(T element)
        {
            _elements.Remove(element);
            OnValueChanged?.Invoke();
        }

        public void RemoveElement(int index)
        {
            _elements.RemoveAt(index);
            OnValueChanged?.Invoke();
        }

        public void SetTo(List<T> elements)
        {
            _elements = elements;
            OnValueChanged?.Invoke();
        }

        public void Clear()
        {
            _elements = new List<T>();
            OnValueChanged?.Invoke();
        }

        public int Count()
        {
            return _elements.Count;
        }

        public List<T> GetCopyOfElements()
        {
            return new List<T>(_elements);
        }

        public T GetElement(int index)
        {
            return _elements[index];
        }
    }
}