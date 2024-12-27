using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Darkan.Utilities
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return (layerMask & (1 << layer)) != 0;
        }

        /// <summary>
        /// Shuffles a list. Good performance. Meh randomness.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
            return list;
        }

        /// <returns>The world position of the RectTransform relative to the Camera</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WorldPosition(this RectTransform rectTrans, Camera camera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, rectTrans.position, camera, out Vector3 result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color SetAlpha(this Color thisColor, float alpha)
        {
            thisColor.a = alpha;
            return thisColor;
        }

        /// <returns>Returns a Vector2 with the x and z components of the Vector3 and normalized</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 FlatVec2Normal(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z).normalized;
        }

        /// <returns>Returns the input Vector with y = 0 and normalized</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FlatNormal(this Vector3 vector3)
        {
            return new Vector3(vector3.x, 0, vector3.z).normalized;
        }

        /// <returns>
        /// the input Vector with y = 0 and the original magnitude
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Flat(this Vector3 vector3)
        {
            float magnitude = vector3.magnitude;
            vector3 = FlatNormal(vector3);
            return vector3 * magnitude;
        }

        /// <returns>
        /// the input Vector with y = 0 and the original x and z
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FlatPos(this Vector3 vector3)
        {
            vector3.y = 0;
            return vector3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampMin(this float input, float minValue)
        {
            return input < minValue ? minValue : input;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampMin(this int input, int minValue)
        {
            return input < minValue ? minValue : input;
        }
    }
}