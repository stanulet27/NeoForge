/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.Text.RegularExpressions;
using UnityEngine;

namespace SharedData
{
    /// <summary>
    /// A string that is able to be shared between multiple objects
    /// </summary>
    [CreateAssetMenu(menuName = "Shared Data/String", fileName = "New Shared String")]
    public class SharedString : SharedDataBase<string>
    {
        [SerializeField] private string _value;
        [SerializeField, Tooltip("Regex Expression")] private string _requiredFormat = "";

        public override string Value
        {
            get => _value;
            set
            {
                _value = _requiredFormat == "" || new Regex(_requiredFormat).IsMatch(value) ? value : _value;
                BroadcastValueChanged();
            }
        }
        
        public void AppendToString(string append)
        {
            Value += append;
        }

        public void Backspace()
        {
            if(_value.Length > 0) Value = _value[..^1];
        }

        public override string ToString()
        {
            return _value;
        }
    }
}