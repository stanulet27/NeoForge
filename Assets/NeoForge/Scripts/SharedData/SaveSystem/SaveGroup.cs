/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2022, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SharedData.SaveSystem
{
    /// <summary>
    /// Holds several lists of shared data that will be used to save and load data
    /// </summary>
    [CreateAssetMenu(fileName = "New Data to Save", menuName = "Save Group")]
    public class SaveGroup : ScriptableObject
    {
        [SerializeField] private List<SharedListBase<string>> stringLists;
        [SerializeField] private List<SharedDataBase<string>> strings;
        [SerializeField] private List<SharedDataBase<int>> ints;
        [SerializeField] private List<SharedDataBase<bool>> booleans;

        public SaveData SaveData => new()
        {
            stringLists = stringLists.ConvertAll(x => new NestedList<string>(x)),
            strings = strings.ConvertAll(x => x.Value),
            ints = ints.ConvertAll(x => x.Value),
            booleans = booleans.ConvertAll(x => x.Value)
        };

        public bool ValidateData(SaveData data)
        {
            return ValidateData(stringLists, data.stringLists)
                && ValidateData(strings, data.strings)
                && ValidateData(ints, data.ints)
                && ValidateData(booleans, data.booleans);
        }

        public void LoadData(SaveData data)
        {
            Load(stringLists, data.stringLists);
            Load(strings, data.strings);
            Load(ints, data.ints);
            Load(booleans, data.booleans);
        }

        private static bool ValidateData(ICollection references, ICollection values)
        {
            return references.Count == values.Count;
        }

        private void Load<T>(List<SharedListBase<T>> references, List<NestedList<T>> values)
        {
            Debug.Assert(references.Count == values.Count);

            for (var i = 0; i < references.Count; i++)
            {
                if (references[i].GetCopyOfElements().Equals(values[i].list)) continue;

#if UNITY_EDITOR
                EditorUtility.SetDirty(references[i]);
#endif

                references[i].SetTo(values[i].list);
            }
        }

        private void Load<T>(List<SharedDataBase<T>> references, List<T> values)
        {
            Debug.Assert(references.Count == values.Count);

            for (var i = 0; i < references.Count; i++)
            {
                if (references[i].Value.Equals(values[i])) continue;

#if UNITY_EDITOR
                EditorUtility.SetDirty(references[i]);
#endif

                references[i].Value = values[i];
            }
        }
    }
}