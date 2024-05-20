/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using UnityEngine;
using System.Linq;

namespace CustomInspectors
{
    /// <summary>
    /// This class is used as a wrapper for a reference type so that it can be serialized in the Unity Inspector.
    /// Mainly meant for reference types that are not usually visible within the unity inspector.
    /// </summary>
    /// <typeparam name="ReferenceType">The type of the reference</typeparam>
    [System.Serializable]
    public class SerializedReference<ReferenceType> : ISerializationCallbackReceiver where ReferenceType : class
    {
        public Object target;
        public ReferenceType Reference => target as ReferenceType;
        public static implicit operator bool(SerializedReference<ReferenceType> self) => self.target != null;
        
        void ValidateSerialization()
        {
            if (target is GameObject g and not ReferenceType)
            {
                target = g.GetComponents<Component>().ToList().Find(component => component is ReferenceType);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() => ValidateSerialization();

        void ISerializationCallbackReceiver.OnAfterDeserialize() {}
    }
}