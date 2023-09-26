using System.Collections.Generic;
using UnityEngine;

namespace Darkan.Tools
{
    public class Tools : MonoBehaviour
    {
        /// <summary>Invoke every second, or when necessary, for better performance</summary>
        /// <returns>String in format mm:ss with m and s rounded down</returns>
        public static string GetTimerString(float timer)
        {
            float minutes = (int)(timer / 60);
            float seconds = (int)(timer % 60);

            return (minutes * 100 + seconds).ToString("00:00");
        }

        /// <summary>
        /// Divide a vector into equal parts. Non alloc.
        /// </summary>
        /// <returns>A list with all parts, set off by startpos and distance.</returns>
        public static void DivideVectorIntoEqualParts(float startPos, float distance, int parts, List<float> result)
        {
            result.Clear();

            float partSize = distance / parts;

            for (int i = 0; i < parts; i++)
            {
                result.Add(startPos + i * partSize);
            }
        }
    }
}
