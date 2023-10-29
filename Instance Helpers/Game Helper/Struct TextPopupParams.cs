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

        /// <summary>
        /// Text = string.Empty <br/>
        /// Color = Color.white <br/>
        /// Duration = 1.75 <br/>
        /// Distance = 0.15 <br/>
        /// FadeTime = 0.35 <br/>
        /// FontSize = 8.5 <br/>
        /// WorldPos = Vector3.zero
        /// </summary>
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
                    Distance = .15f,
                    FadeTime = .35f,
                    FontSize = 8.5f,
                    WorldPos = Vector3.zero
                };
            }
        }
    }
}
