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
using SpeechProcessing;

namespace MenuSystems.SpeechProcessing
{
    /// <summary>
    /// Represents a speech command that can be given by a user. Includes a the command identifying label, any options
    /// selected for that command, any values given for that command, and whether that command is valid
    /// </summary>
    public struct SpeechCommand
    {
        public SpeechLabel Label;
        public List<string> OptionsSelected;
        public bool HasOption;
        public int NumberValue;
        public bool HasNumber;
        public bool IsValid;

        public override string ToString()
        {
            var s = SpeechHelperClass.SpeechLabelToString(Label);
            s = char.ToUpper(s[0]) + s[1..];
            if (HasOption)
            {
                OptionsSelected.Where(x => x != "").ToList().ForEach(x => s += " " + x + " and");
                s = s.Remove(s.Length - 4);
            }
            s += HasNumber ? " by " + NumberValue : "";
            
            return s;
        }
    }
}