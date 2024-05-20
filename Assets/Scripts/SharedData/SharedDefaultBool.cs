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
    public class SharedDefaultBool : SharedData<bool>
    {
        [SerializeField] private bool value;
        [SerializeField] private bool defaultValue;

        public override bool Value
        {
            get => value;
            set
            {
                this.value = value;
                BroadcastValueChanged();
            }
        }

        private void OnEnable()
        {
            Value = defaultValue;
            Debug.Log("Set To Default");
        }

        [ContextMenu("Flip boolean")]
        public void Flip()
        {
            Value = !Value;
        }
    }
}