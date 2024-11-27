using UnityEngine;

namespace Darkan.Internal
{
    internal class PersistentSystemsLoader : Object
    {
        const string DIRECTORY_NAME = "PersistentSystems";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Object[] resources = Resources.LoadAll(DIRECTORY_NAME, typeof(GameObject));

            foreach (Object resource in resources)
            {
                GameObject prefabToLoad = resource as GameObject;

                if (prefabToLoad == null) continue;

                GameObject instance = Instantiate(prefabToLoad);
                DontDestroyOnLoad(instance);
            }
        }
    }
}