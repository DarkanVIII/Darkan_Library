namespace Darkan.UI
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;

    public abstract class CanvasBase : MonoBehaviour
    {
        [SerializeField] StyleSheet _styleSheet;

        protected VisualElement Root;

        void Awake()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            Root.styleSheets.Add(_styleSheet);

            StartCoroutine(BuildCanvas());
        }

        void OnValidate()
        {
            if (Application.isPlaying) return;
            StartCoroutine(ResetRoot());
        }

        IEnumerator ResetRoot() //Used in Editor to Update Canvas, Waits for next frame because Root is null when leving playmode in Editor (Unity Bug)
        {
            yield return null;
            Root = GetComponent<UIDocument>().rootVisualElement;
            Root.Clear();
            Root.styleSheets.Add(_styleSheet);

            StartCoroutine(BuildCanvas());
        }

        protected abstract IEnumerator BuildCanvas();
    }
}
