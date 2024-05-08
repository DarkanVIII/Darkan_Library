using System.Runtime.CompilerServices;
using UnityEngine;

namespace Darkan
{
    public static class Mathd
    {
        /// <summary>
        /// Only use dot with 2 normalized Vectors.
        /// </summary>
        /// <returns>Radians up to PI</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DotToRadians(float dot)
        {
            dot = Mathf.Clamp(dot, -1f, 1f);
            return Mathf.Acos(dot);
        }

        /// <summary>
        /// Only use dot with 2 normalized Vectors.
        /// </summary>
        /// <returns>Degrees up to 180°</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DotToDegrees(float dot)
        {
            return DotToRadians(dot) * Mathf.Rad2Deg;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RadiansToDot(float radians)
        {
            return Mathf.Cos(radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DegreesToDot(float degrees)
        {
            return Mathf.Cos(degrees * Mathf.Deg2Rad);
        }

        /// <summary>
        /// Starts from y = 1 goes counterclockwise to x = -1; y = -1; x = 1
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 AngleToDirection(float angleInRadians)
        {
            return new Vector2(Mathf.Sin(-angleInRadians), Mathf.Cos(angleInRadians));
        }
    }
}
