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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadToDot(float radians)
        {
            return Mathf.Cos(radians);
        }

        /// <summary>
        /// Starts from y = 1, x = 0
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AngleToFrwDir(float angleInRad)
        {
            return new Vector2(Mathf.Sin(angleInRad), Mathf.Cos(angleInRad));
        }

        /// <summary>
        /// Starts from y = 1, x = 0
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FrwDirToAngle(Vector2 dir)
        {
            return Mathf.Atan2(dir.x, dir.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DirToAngle(Vector2 dir)
        {
            return Mathf.Atan2(dir.y, dir.x);
        }

        /// <summary>
        /// "Determinant" or "Wedge Product" or "Perpendicular Dot Product"<br></br>
        /// Wedge/2 = signed area of triangle
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Wedge(Vector2 a, Vector2 b) => a.x * b.y - a.y * b.x;
    }
}