/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.Diagnostics.CodeAnalysis;

namespace CommandSystem
{
    /// <summary>
    /// An enum of possible speech labels
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CommandLabel
    {
        Open = 1,
        Close = 2,
        Increase = 3,
        Decrease = 4,
        Set = 5,
        NextPage = 6,
        PreviousPage = 7,
        Switch = 8,
        Toggle = 9,
        ClearSelections = 10,
        Confirm = 11,
        Home = 12,
        ExitApplication = 13,
        Cancel = 14,
        FastForward = 15,
        SlowDown = 16,
        Play = 17,
        Pause = 18,
        
        Select = 20,
        Next = 21,
        Previous = 22,
        Rotate = 23,
        
        Start = 25,
        Stop = 26,
        Submit = 27,
        Log = 28,
        Click = 29,

        Delete = 30,
        Create = 31,
        Yes = 32,
        No = 33,
        Download = 34,
        Modify = 35,
        Reset = 36,

        Never_Mind = 100
    }
}