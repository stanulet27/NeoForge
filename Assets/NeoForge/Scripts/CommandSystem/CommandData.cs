/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace MenuSystems.SpeechProcessing
{
    /// <summary>
    /// Represents a speech command that can be given by a user. Includes the label to specify this command as well as
    /// alternative label options and the options that the user can specify regarding this command, both numerical and
    /// non-numerical.
    /// </summary>
    [System.Serializable]
    public class CommandData
    {
        public string Label => CommandHelperClass.SpeechLabelToString(CommandLabel);
        
        public CommandLabel CommandLabel;
        public List<string> PossibleAlternatives = new();
        public List<string> ValidOptions = new();
        public bool HasOptions => ValidOptions != null && ValidOptions.Count != 0;
        public bool HasNumericValue;

        public override string ToString()
        {
            return $"Label: {Label} " +
                $"| Possible Alternatives: {PossibleAlternatives.Aggregate("", (x, y) => x + ", " + y)} " +
                $"| Valid Options: {ValidOptions.Aggregate("",(x, y) => x + ", " + y)} " +
                $"| Has Numeric Value: {HasNumericValue}";
        }

        /// <summary>
        /// Updates the command data with the given option and variations. If the option is not already in the list of
        /// valid options then it will be added. If the variations are not already in the list of possible alternatives
        /// then they will be added.
        ///
        /// Requires: variations != null
        /// </summary>
        /// <param name="option"></param>
        /// <param name="variations"></param>
        public void Update(string option, List<string> variations)
        {
            Debug.Assert(variations != null, "Variations cannot be null");
            
            if (!ValidOptions.Contains(option) && option != "")
            {
                ValidOptions.Add(option);
            }
            
            PossibleAlternatives = PossibleAlternatives.Union(variations).ToList();
        }
    }
}