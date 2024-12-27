using System.Collections.Generic;
using UnityEngine;

namespace Darkan.Utilities
{
    public static class Helpers
    {
        static readonly Dictionary<float, WaitForSeconds> _waitDict = new();

        /// <summary>
        /// Caches each WaitForSeconds to reduce allocations.<br/>
        /// Only use when using the same or very limited time values with each call to 
        /// keep internal dictionary size small.
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            if (_waitDict.TryGetValue(seconds, out WaitForSeconds wait)) return wait;

            wait = new(seconds);
            _waitDict[seconds] = wait;
            return wait;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ClearWaitDictionary()
        {
            _waitDict.Clear();
        }

        /// <summary>Invoke every second, or when necessary, for better performance</summary>
        /// <returns>String in format mm:ss with m and s rounded down</returns>
        public static string GetTimerString(float timerInSec)
        {
            float minutes = (int)(timerInSec / 60);
            float seconds = (int)(timerInSec % 60);

            return (minutes * 100 + seconds).ToString("00:00");
        }

        /// <returns>A list with all parts</returns>
        public static void DivideVectorIntoEqualParts(float origin, float distance, int parts, List<float> result)
        {
            result.Clear();

            float partSize = distance / parts;

            for (int i = 0; i < parts; i++)
            {
                result.Add(origin + i * partSize);
            }
        }
    }
}