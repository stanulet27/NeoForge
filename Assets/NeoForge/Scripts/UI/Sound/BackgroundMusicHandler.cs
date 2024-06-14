using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NeoForge.UI.Sound
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(VolumeMatcher))]
    public class BackgroundMusicHandler : MonoBehaviour
    {
        [Tooltip("The music tracks to play in order.")]
        [SerializeField] private List<AudioClip> _musicTracks;
        [Tooltip("Determines whether to have a random song follow after the current one.")]
        [SerializeField] private bool _shuffle;
        [Tooltip("The delay between tracks.")]
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

            StartCoroutine(HandleQueue());
        }
        
        private IEnumerator HandleQueue()
        {
            while (true)
            {
                _audioSource.Play();
                yield return new WaitForSeconds(CurrentTrack.length);
                _audioSource.Stop();

                SwapTrack();

                yield return new WaitForSeconds(_delayBetweenTracks);
            }
        }

        private void SwapTrack()
        {
            _currentTrackIndex = _shuffle
                ? GetRandomEntry(_musicTracks, _currentTrackIndex)
                : (_currentTrackIndex + 1) % _musicTracks.Count;
            _audioSource.clip = CurrentTrack;
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