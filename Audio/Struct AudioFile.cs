using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Darkan.Audio
{
    [Serializable]
    public struct AudioFile
    {
        public AudioClip AudioClip;
        [Range(0f, 1f)]
        public float Volume;
        public bool PlayOnAwake;
        public bool Loop;
        public bool Is3D;
        [ShowIf("Is3D")]
        [Tooltip("Set this to the distance the sound will start falling off.")]
        public float MinDistance;
        [ShowIf("Is3D")]
        [Tooltip("The max distance the sound can be heard at. Affects how fast the sound falls off.")]
        [ShowIf("Is3D")]
        public float MaxDistance;
        [ShowIf("Is3D")]
        public Transform SourceTransform;
        [HideInInspector] // Used internally for setting default values in the inspector
        public bool OldField;

        public static readonly AudioFile Default = new()
        {
            PlayOnAwake = true,
            Volume = 1f,
            OldField = true,
        };
    }
}