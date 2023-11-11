using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Darkan.Audio
{
    public class AudioManager : SerializedMonoBehaviour
    {
#if !RELEASE
        public static AudioManager I { get; private set; }
#else
        public static AudioManager I;
#endif
        [OnCollectionChanged(After = "SetDefaultAudioFile")]
        [SerializeField] Dictionary<string, AudioFile> _audioFiles = new();
        Dictionary<string, AudioSource> _audioSources;

        void Awake()
        {
            I = this;

            _audioSources = new Dictionary<string, AudioSource>(_audioFiles.Count);

            CreateAudioSources();
        }

        void Start()
        {
            foreach (KeyValuePair<string, AudioSource> pair in _audioSources)
            {
                if (pair.Value.playOnAwake)
                {
                    pair.Value.Play();
                }
            }
        }

        //Used by Odin Inspector, and is called on collection changed
        void SetDefaultAudioFile(CollectionChangeInfo changeInfo)
        {
            if (changeInfo.ChangeType is CollectionChangeType.SetKey)
                if (!_audioFiles[(string)changeInfo.Key].OldField)
                    _audioFiles[(string)changeInfo.Key] = AudioFile.Default;
        }

        void CreateAudioSources()
        {
            foreach (KeyValuePair<string, AudioFile> pair in _audioFiles)
            {
                if (pair.Value.Is3D)
                {
                    GameObject go = new("3D Audio Source_" + pair.Key);
                    go.transform.parent = pair.Value.SourceTransform;
                    go.transform.localPosition = Vector3.zero;

                    AudioSource audioSource = go.AddComponent<AudioSource>();
                    _audioSources.Add(pair.Key, audioSource);
                    SetupAudioSource(pair.Value, audioSource);
                }
                else
                {
                    GameObject go = new("2D Audio Source_" + pair.Key);
                    go.transform.parent = transform;

                    AudioSource audioSource = go.AddComponent<AudioSource>();
                    _audioSources.Add(pair.Key, audioSource);
                    SetupAudioSource(pair.Value, audioSource);
                }
            }
        }

        /// <summary>
        /// Plays the audio clip specified by "clipID".<br/>
        /// Overplay: Restarts the clip if it was already playing.<br/>
        /// No Overplay: Won't play if the clip is already playing.
        /// </summary>
        public void Play(string clipID, bool overplay = false)
        {
            if (_audioSources.TryGetValue(clipID, out AudioSource audioSource))
            {
                if (overplay)
                    audioSource.Play();
                else if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            else Debug.LogWarning($"Audio Clip with ID {clipID} could not be found.");
        }

        /// <summary>
        /// Finds and stops the audio clip specified by "clipName".
        /// </summary>
        public void Stop(string clipID)
        {
            if (_audioSources.TryGetValue(clipID, out AudioSource audioSource))
            {
                audioSource.Stop();
            }
            else Debug.LogWarning($"Audio Clip with ID {clipID} could not be found.");
        }

        void SetupAudioSource(AudioFile audioFile, AudioSource audioSource)
        {
            audioSource.clip = audioFile.AudioClip;
            audioSource.volume = audioFile.Volume;
            audioSource.playOnAwake = audioFile.PlayOnAwake;
            audioSource.loop = audioFile.Loop;

            if (audioFile.Is3D)
            {
                audioSource.spatialBlend = 1;
                audioSource.maxDistance = audioFile.MaxDistance;
                audioSource.minDistance = audioFile.MinDistance;
            }
        }
    }
}
