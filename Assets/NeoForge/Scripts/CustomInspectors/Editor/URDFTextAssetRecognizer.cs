/// Software License Agreement (Apache 2.0 License)
///
/// Copyright (c) 2021-2023, The Ohio State University
/// Center for Design and Manufacturing Excellence (CDME)
/// The Artificially Intelligent Manufacturing Systems Lab (AIMS)
/// All rights reserved.
///
/// Author: Chase Oberg

using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace CustomInspectors.Editor
{
    /// <summary>
    /// This class will enabled files with the extension .urdf to be recognized as TextAsset files by the Unity Asset Importer.
    /// </summary>
    [ScriptedImporter(1, "urdf")]
    public class URDFTextAssetRecognizer : ScriptedImporter 
    {
        public override void OnImportAsset(AssetImportContext asset) 
        {
            var subAsset = new TextAsset(File.ReadAllText( asset.assetPath));
            asset.AddObjectToAsset("text", subAsset);
            asset.SetMainObject(subAsset);
        }
    }
}

