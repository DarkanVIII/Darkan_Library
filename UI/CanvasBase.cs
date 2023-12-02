namespace Darkan.UI
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;

    [ExecuteAlways]
    public abstract class CanvasBase : MonoBehaviour
    {
        [SerializeField] StyleSheet _styleSheet;

        protected UIDocument UIDocument;
        protected VisualElement Root;

        void Awake()
        {
            UIDocument = GetComponent<UIDocument>();
            Root = UIDocument.rootVisualElement;
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

        protected T Create<T>(VisualElement parent, params string[] classNames) where T : VisualElement, new()
        {
            T element = new();

            if (parent != null)
                parent.Add(element);

            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }

        protected T Create<T>(params string[] classNames) where T : VisualElement, new()
        {
            T element = new();

            foreach (string className in classNames)
            {
                element.AddToClassList(className);
            }

            return element;
        }
    }
}
