namespace Darkan.RuntimeTools
{
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [System.Serializable]
    public struct TextPopupParams
    {
        public TextPopupParams(TextPopupParams paramPreset = default)
        {
            Text = paramPreset.Text;
            Color = paramPreset.Color;
            Duration = paramPreset.Duration;
            Distance = paramPreset.Distance;
            FadeTime = paramPreset.FadeTime;
            FontSize = paramPreset.FontSize;
            StartPos = paramPreset.StartPos;
        }

        [HideInInspector]
        public string Text;
        public Color Color;
        public float Duration;
        public Vector3 Distance;
        public float FadeTime;
        public float FontSize;
        public Vector3 StartPos;

        public static TextPopupParams BasicWhite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new()
                {
                    Text = string.Empty,
                    Color = Color.white,
                    Duration = 1.75f,
                    Distance = new(0, .3f, 0),
                    FadeTime = .35f,
                    FontSize = 8.5f,
                };
            }
        }
    }
}
