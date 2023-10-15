namespace Darkan.Extensions
{
    using GameHelper;
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

        /// <returns>The world position of the RectTransform using the Main Camera</returns>
        public static Vector3 WorldPosition(this RectTransform rectTrans)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTrans, rectTrans.position, GameHelper.MainCamera, out Vector3 result);
            return result;
        }
    }
}

