namespace Darkan.Helpers
{
    using Darkan.RuntimeTools;
    using System.Collections.Generic;
    using UnityEngine;

    public static class Extensions
    {
        /// <returns>Integer with the provided ratio as percentage</returns>
        public static int ApplyPercent(this int integer, int divident, int divisor)
        {
            return System.Convert.ToInt32((float)divident / divisor * integer);
        }

        /// <returns>Float with the provided ratio as percentage</returns>
        public static float ApplyPercent(this float floating, int divident, int divisor)
        {
            return divident / divisor * floating;
        }

        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return (layerMask & (1 << layer)) != 0;
        }

        /// <summary>
        /// Shuffles a list. Good performance. Meh randomness.
        /// </summary>
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
        public static Vector3 WorldPosition(this RectTransform rectTrans, Camera camera)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, rectTrans.position, camera, out Vector3 result);
            return result;
        }
    }
}

