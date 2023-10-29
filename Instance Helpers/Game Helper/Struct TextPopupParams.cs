using System.Runtime.CompilerServices;
using UnityEngine;

namespace Darkan.GameHelper
{
    public struct TextPopupParams
    {
        public string Text;
        public Color Color;
        public float Duration;
        public float Distance;
        public float FadeTime;
        public float FontSize;
        public Vector3 WorldPos;

        public static TextPopupParams BasicWhite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new()
                {
                    Text = string.Empty,
                    Color = Color.white,
                    Duration = 1.6f,
                    Distance = .3f,
                    FadeTime = .35f,
                    FontSize = 8.5f,
                    WorldPos = Vector3.zero
                };
            }
        }
    }
}
