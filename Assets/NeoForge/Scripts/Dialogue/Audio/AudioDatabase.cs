using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace NeoForge.Dialogue.Audio
{
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "AudioDatabase")]
    public class AudioDatabase : ScriptableObject
    {
        public SerializedDictionary<string, AudioClip> AudioClips = new();
    }
}