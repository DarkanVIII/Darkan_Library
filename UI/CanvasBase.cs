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

        protected VisualElement Create(params string[] classNames)
        {
            return Create<VisualElement>(null, classNames);
        }

        protected VisualElement Create(VisualElement parent, params string[] classNames)
        {
            return Create<VisualElement>(parent, classNames);
        }

        protected VisualElement Create(string className, VisualElement parent = null)
        {
            return Create<VisualElement>(parent, className);
        }

        protected T Create<T>(VisualElement parent, params string[] classes) where T : VisualElement, new()
        {
            T element = new();

            if (parent != null)
                parent.Add(element);

            foreach (string className in classes)
            {
                element.AddToClassList(className);
            }

            return element;
        }

        protected T Create<T>(params string[] classes) where T : VisualElement, new()
        {
            T element = new();

            foreach (string className in classes)
            {
                element.AddToClassList(className);
            }

            return element;
        }
    }
}
