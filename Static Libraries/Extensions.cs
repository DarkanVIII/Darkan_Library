namespace Darkan.Helpers
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class Extensions
    {
        /// <returns>Integer with the provided ratio as percentage</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ApplyPercent(this int integer, int divident, int divisor)
        {
            return System.Convert.ToInt32((float)divident / divisor * integer);
        }

        /// <returns>Float with the provided ratio as percentage</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ApplyPercent(this float floating, int divident, int divisor)
        {
            return divident / divisor * floating;
        }

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

        /// <summary>
        /// Returns a Vector2 with the x and z components of the Vector3 and normalized
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 FlatVec2Normalized(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z).normalized;
        }

        /// <summary>
        /// Returns the input Vector with y = 0 and normalized
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FlatNormalized(this Vector3 vector3)
        {
            return new Vector3(vector3.x, 0, vector3.z).normalized;
        }

        /// <summary>
        /// Returns the input Vector with y = 0 and the original magnitude
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Flat(this Vector3 vector3)
        {
            float magnitude = vector3.magnitude;
            vector3 = FlatNormalized(vector3);
            return vector3 * magnitude;
        }

        /// <summary>
        /// Returns the input Vector with y = 0 and the original x and z
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 FlatPos(this Vector3 vector3)
        {
            vector3.y = 0;
            return vector3;
        }

        /// <summary>
        /// Value is clamped to a min value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ClampMin(this float input, float minValue)
        {
            if (input < minValue)
                return minValue;

            return input;
        }

        /// <summary>
        /// Value is clamped to a min value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ClampMin(this int input, int minValue)
        {
            if (input < minValue)
                return minValue;

            return input;
        }
    }
}

