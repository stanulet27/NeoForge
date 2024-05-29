/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using UnityEngine;

namespace SharedData
{
    /// <summary>
    ///     This serves as scriptable object that can be used to have a shared bool between objects. It also
    ///     contains a default value that will be set when the object is enabled.
    /// </summary>
    [CreateAssetMenu(fileName = "New Shared Bool", menuName = "Shared Data/Defaultable/Bool")]
    public class SharedDefaultBool : SharedDataBase<bool>
    {
        [SerializeField] private bool _value;
        [SerializeField] private bool _defaultValue;

        public override bool Value
        {
            get => _value;
            set
            {
                this._value = value;
                BroadcastValueChanged();
            }
        }

        private void OnEnable()
        {
            Value = _defaultValue;
            Debug.Log("Set To Default");
        }

        [ContextMenu("Flip boolean")]
        public void Flip()
        {
            Value = !Value;
        }
    }
}