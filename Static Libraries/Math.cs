using System.Runtime.CompilerServices;
using UnityEngine;

namespace Darkan
{
    public struct MathD
    {
        /// <summary>
        /// PI*2 | A full turn in Radians (Unit Circle)
        /// </summary>
        public const float TAU = 6.283185307f;
        public const float Turn2Rad = TAU;
        public const float Rad2Turn = 0.15915494f;

        /// <summary>
        /// Only works with DOT of 2 normalized Vectors.
        /// </summary>
        /// <returns>Up to half a Turn in Radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DotToRad(float dot)
        {
            dot = Mathf.Clamp(dot, -1f, 1f);
            return Mathf.Acos(dot);
        }

        /// <summary>
        /// Only works with DOT of 2 normalized Vectors.
        /// </summary>
        /// <returns>Up to half a Turn in degrees</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DotToDeg(float dot)
        {
            return DotToRad(dot) * Mathf.Rad2Deg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadToDot(float radians)
        {
            return Mathf.Cos(radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegToDot(float degrees)
        {
            return Mathf.Cos(degrees * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Starts from y = 1 goes counterclockwise to x = -1; y = -1; x = 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AngleToFrwDir(float angleInRad)
        {
            return new Vector2(Mathf.Sin(-angleInRad), Mathf.Cos(angleInRad));
        }
    }
}