using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoForge.UI.Sound
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(VolumeMatcher))]
    public class BackgroundMusicHandler : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> _musicTracks;
        [SerializeField] private bool _shuffle;
        [SerializeField] private float _delayBetweenTracks = 1f;

        private AudioSource _audioSource;
        private int _currentTrackIndex;
        private AudioClip CurrentTrack => _musicTracks[_currentTrackIndex];

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _audioSource.loop = false;
            _currentTrackIndex = GetRandomEntry(_musicTracks);
            _audioSource.clip = CurrentTrack;
            _audioSource.Play();

            StartCoroutine(HandleQueue());
        }
        
        private IEnumerator HandleQueue()
        {
            while (true)
            {
                yield return new WaitForSeconds(CurrentTrack.length);
                _audioSource.Stop();

                _currentTrackIndex = _shuffle 
                    ? GetRandomEntry(_musicTracks, _currentTrackIndex) 
                    : (_currentTrackIndex + 1) % _musicTracks.Count;

                _audioSource.clip = CurrentTrack;
                yield return new WaitForSeconds(_delayBetweenTracks);
                _audioSource.Play();
            }
        }

        private static int GetRandomEntry<T>(List<T> entries, int indexToExclude = -1)
        {
            if (entries.Count == 0) return 0;
            
            var copyOfEntries = new List<T>(entries);
            if (indexToExclude != -1) copyOfEntries.RemoveAt(indexToExclude);
            var randomIndex = Random.Range(0, copyOfEntries.Count);
            return entries.IndexOf(copyOfEntries[randomIndex]);
        }
    }
}