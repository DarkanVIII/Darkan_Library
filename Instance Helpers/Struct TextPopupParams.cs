namespace Darkan.InstanceHelpers
{
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public struct TextPopupParams
    {
        public string Text;
        public Color Color;
        public float Duration;
        public Vector3 Distance;
        public float FadeTime;
        public float FontSize;
        public Vector3 Offset;

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
                    Offset = Vector3.zero,
                };
            }
        }
    }
}
